using AutoFixture;
using FluentAssertions;
using SerializerTests.Nodes;
using System.Text;

namespace ListSerializer.Tests
{
    public class ListSerializerTest
    {

        [Fact]
        public async Task DeepCopy_SingleNode_SameObject()
        {
            // Arrange
            var fixture = new Fixture();

            var suts = fixture
                .Build<ListNode>()
                .Without(x => x.Next)
                .Without(x => x.Previous)
                .Without(x => x.Random)
                .CreateMany(2);

            var sut = suts.First();

            sut.Next = suts.Skip(1).First();
            sut.Random = suts.Skip(1).First();
            sut.Next.Random = suts.First();
            sut.Next.Previous = sut;

            var serializer = new ListSerializer();

            // Act
            var result = await serializer.DeepCopy(sut);

            // Assert
            result.Data.Should().BeEquivalentTo(sut.Data);
        }

        [Fact]
        public async Task DeepCopy_NodeWithNext_SameObject()
        {
            // Arrange
            var fixture = new Fixture();

            var suts = fixture
                .Build<ListNode>()
                .Without(x => x.Next)
                .Without(x => x.Previous)
                .Without(x => x.Random)
                .CreateMany(2);

            var sut = suts.First();

            sut.Next = suts.Skip(1).First();
            sut.Random = suts.Skip(1).First();
            sut.Next.Random = suts.First();
            sut.Next.Previous = sut;

            var serializer = new ListSerializer();

            // Act
            var result = await serializer.DeepCopy(sut);

            // Assert
            result.ToFullString().Should().BeEquivalentTo(sut.ToFullString());
        }

        [Fact]
        public async Task DeepCopy_NodeWithNextAndRandom_SameObject()
        {
            // Arrange
            var fixture = new Fixture();

            var suts = fixture
                .Build<ListNode>()
                .Without(x => x.Next)
                .Without(x => x.Previous)
                .Without(x => x.Random)
                .CreateMany(2);

            var sut = suts.First();

            sut.Next = suts.Skip(1).First();
            sut.Random = suts.Skip(1).First();
            sut.Next.Random = suts.First();
            sut.Next.Previous = sut;

            var serializer = new ListSerializer();

            // Act
            var result = await serializer.DeepCopy(sut);

            // Assert
            result.ToFullString().Should().BeEquivalentTo(sut.ToFullString());
        }

        [Fact]
        public async Task DeepCopy_NodeWithCycleLink_SameObject()
        {
            // Arrange
            var fixture = new Fixture();

            var suts = fixture
                .Build<ListNode>()
                .Without(x => x.Next)
                .Without(x => x.Previous)
                .Without(x => x.Random)
                .CreateMany(2);

            var sut = suts.First();

            sut.Next = suts.Skip(1).First();
            sut.Random = suts.Skip(1).First();
            sut.Next.Random = suts.First();
            sut.Next.Previous = sut;
            sut.Next.Next = suts.First();

            var serializer = new ListSerializer();

            // Act
            var result = await serializer.DeepCopy(sut);

            // Assert
            result.ToFullString().Should().BeEquivalentTo(sut.ToFullString());
        }

        [Fact]
        public async Task DeepCopy_NodeWithUnicodeSymbol_SameObject()
        {
            // Arrange
            var fixture = new Fixture();

            var suts = fixture
                .Build<ListNode>()
                .Without(x => x.Next)
                .Without(x => x.Previous)
                .Without(x => x.Random)
                .CreateMany(2);

            var sut = suts.First();

            sut.Next = suts.Skip(1).First();
            sut.Random = suts.Skip(1).First();
            sut.Next.Random = suts.First();
            sut.Next.Previous = sut;

            // Inject in string unicode symbol
            sut.Data = "Maths use \u03a0 (Pi) for calculations";

            var serializer = new ListSerializer();

            // Act
            var result = await serializer.DeepCopy(sut);

            // Assert
            result.ToFullString().Should().BeEquivalentTo(sut.ToFullString());
        }

        [Theory]
        [InlineData(10)]
        public async Task ListSerializer_SerializedAndDesialized_ListEqual(int countNode)
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

            // Assert
            while (result.Next != null || head.Next != null)
            {
                Assert.Equal(result.Data, head.Data);
                Assert.Equal(result.Previous?.Data, head.Previous?.Data);
                Assert.Equal(result.Next?.Data, head.Next?.Data);

                result = result?.Next;
                head = head?.Next;
            }

            // Check end list
            Assert.Equal(result.Next, head.Next);
        }
    }
}