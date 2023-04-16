using AutoFixture;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using SerializerTests.Interfaces;
using SerializerTests.Nodes;

namespace ListSerializer.Benchmark
{
    internal class Program
    {
        public static void Main()
        {
            BenchmarkRunner
                .Run<SerializeBenchmarkJob>();
        }
    }

    [RankColumn(BenchmarkDotNet.Mathematics.NumeralSystem.Stars)]
    [ShortRunJob(RuntimeMoniker.Net60)]
    [MemoryDiagnoser]
    [MarkdownExporterAttribute.GitHub]
    public class SerializeBenchmarkJob
    {
        private ListNode head;
        private IEnumerable<SerialiazedObject> serializedObjects;
        private readonly IListSerializer serializer = new ListSerializer();

        [Params(10, 100, 1000)]
        public int Depth { get; set; }

        private ListNode GetTree(int depth)
        {
            var fixture = new Fixture();

            var sut = fixture
                .Build<ListNode>()
                .Without(x => x.Next)
                .Without(x => x.Previous)
                .Without(x => x.Random)
                .CreateMany(depth);

            var rand = new Random();

            sut.First().Previous = null;
            sut.First().Random = sut.ElementAt(rand.Next(0, sut.Count() - 1));
            for (int i = 1; i < sut.Count(); i++)
            {
                sut.ElementAt(i - 1).Next = sut.ElementAt(i);
                sut.ElementAt(i).Previous = sut.ElementAt(i - 1);
                sut.ElementAt(i).Random = sut.ElementAt(rand.Next(0, sut.Count() - 1));
            }

            return sut.First(); ;
        }

        [GlobalSetup]
        public void Setup()
        {
            head = GetTree(Depth);
            var objects = head.ExpandTreeAsync().GetAwaiter().GetResult();
            serializedObjects = objects.ToSerializeData();
        }

        [Benchmark]
        public async Task SerializeWithRandomNode() {
            using (var stream = new MemoryStream())
            {
                await serializer.Serialize(head.Random, stream);
            }
        }

        [Benchmark]
        public async Task SerializeAndDeserialize()
        {
            using (var stream = new MemoryStream())
            {
                await serializer.Serialize(head, stream);
                stream.Seek(0, SeekOrigin.Begin);
                await serializer.Deserialize(stream);
            }
        }

        [Benchmark]
        public async Task DeepCopy()
        {
            var _ = await serializer.DeepCopy(head);
        }

        [Benchmark]
        public async Task DeepCopyWithRandom()
        {
            var _ = await serializer.DeepCopy(head.Random);
        }
    }
}