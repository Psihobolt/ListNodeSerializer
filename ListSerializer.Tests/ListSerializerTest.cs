using AutoFixture;
using FluentAssertions;
using Moq;
using SerializerTests.Interfaces;
using SerializerTests.Nodes;
using System.Collections.Generic;
using Xunit;

namespace ListSerializer.Tests
{
    public class ListSerializerTest
    {
        [Theory]
        [InlineData(10)]
        public async Task ListSerializer_Serialize_ListEqual(int countNode)
        {
            // Arrange
            var fixture = new Fixture();

            var sut = fixture
                .Build<ListNode>()
                .Without(x => x.Next)
                .Without(x => x.Previous)
                .Without(x => x.Random)
                .CreateMany(countNode);

            var rand = new Random();

            sut.First().Previous = null;
            sut.First().Random = sut.ElementAt(rand.Next(0, sut.Count() - 1));
            for (int i = 1; i < sut.Count(); i++)
            {
                sut.ElementAt(i - 1).Next = sut.ElementAt(i);
                sut.ElementAt(i).Previous = sut.ElementAt(i - 1);
                sut.ElementAt(i).Random = sut.ElementAt(rand.Next(0, sut.Count() - 1));
            }


            // Act
            ListNode head = sut.First();
            var serializer = new ListSerializer();

            ListNode result;
            using (var stream = new MemoryStream())
            {
                await serializer.Serialize(head.Random, stream);
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