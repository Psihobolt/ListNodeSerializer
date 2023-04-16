using Moq;
using SerializerTests.Interfaces;
using SerializerTests.Nodes;
using System.Collections.Generic;
using Xunit;

namespace ListSerializer.Tests
{
    public class SerializerTest
    {


        public class ListNodeExt : ListNode
        {
            public override string ToString()
            {
                return String.Join("_",
                    Data,
                    Previous == null ? "Root" : Previous.Data,
                    Next == null ? "End" : Next.Data);
            }

            public override bool Equals(object? obj)
            {
                if (obj == null) return false;
                if (ReferenceEquals(this, obj)) return true;

                return (obj as ListNodeExt).ToString() == (this as ListNodeExt).ToString();
            }

            public override int GetHashCode() => (this as ListNodeExt).ToString().GetHashCode();
        }

        [Theory]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(10000)]
        public async Task ListSerializer_Serialize_ListEqual(int countNode)
        {
            var data = new List<ListNode>();
            data.Add(new ListNodeExt() { Data = "Test data 0", Previous = null });
            for (int i = 1; i < countNode; i++) {
                data.Add(new ListNodeExt() { Data = $"Test data {i}", Previous = data[i-1], Next = null });
                data[i - 1].Next = data[i];
            }

            var rand = new Random();
            var head = data[0];
            while (data.Count > 0) {
                if (data.Count == 1) { 
                    data[0].Random = head; 
                    break; 
                }

                data[0].Random = data[rand.Next(1, data.Count-1)];
                data.Remove(data[0]);
            }

            var serializer = new ListSerializer();
            ListNode result;
            using (var stream = new MemoryStream()) {
                await serializer.Serialize(data[0], stream);
                stream.Seek(0, SeekOrigin.Begin);
                result = await serializer.Deserialize(stream);
            }

            while (result.Next != null || head.Next != null)
            {
                Assert.Equal(result.Data, head.Data);
                Assert.Equal(result.Previous?.Data, head.Previous?.Data);
                Assert.Equal(result.Next?.Data, head.Next?.Data);

                result = result?.Next;
                head = head?.Next;
            }
        }
    }
}