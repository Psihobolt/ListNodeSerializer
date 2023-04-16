
using Newtonsoft.Json;
using SerializerTests.Interfaces;
using SerializerTests.Nodes;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ListSerializer
{
    enum Direction
    {
        Down,
        Up
    }

    public class ListSerializer : IListSerializer
    {
        private record SerialiazedObject
        {
            public int id;
            public int nextId;
            public int prevId;
            public int random;
            public char[]? data;
        }

        public async Task<ListNode> DeepCopy(ListNode head)
        {
            var list = await ExpandTreeAsync(head);
            return ToListNodeData(ToSerializeData(list));
        }

        public Task<ListNode> Deserialize(Stream s)
        {
            if (s != null)
            {
                var serializer = new JsonSerializer();
                IEnumerable<SerialiazedObject>? data;
                using (var stream = new StreamReader(s))
                using (var reader = new JsonTextReader(stream))
                {
                    s.Position = 0;

                    var str = serializer.Deserialize<string>(reader);
                    data = JsonConvert.DeserializeObject<List<SerialiazedObject>>(str);
                }

                return Task.FromResult(ToListNodeData(data));
            }

            return Task.FromResult<ListNode>(null);
        }

        public Task Serialize(ListNode head, Stream s)
        {
            if (s != null)
            {
                var serializer = new JsonSerializer();
                var task = Task.Run(() => ExpandTreeAsync(head));

                var data = task.GetAwaiter().GetResult();

                var str = JsonConvert.SerializeObject(ToSerializeData(data), Formatting.Indented);

                using (var stream = new StreamWriter(s, Encoding.UTF8, str.Length * 2, true))
                using (JsonTextWriter writer = new JsonTextWriter(stream))
                {
                    writer.CloseOutput = false;

                    stream.Flush();
                    s.Position = 0;

                    serializer.Serialize(writer, str);
                }
            }

            return Task.CompletedTask;
        }

        private IEnumerable<SerialiazedObject>? ToSerializeData(IDictionary<int, ListNode> data)
        {
            if (data != null)
            {
                var defaultKeyVal = new KeyValuePair<int, ListNode>(-1, null);

                var count = data.Count;
                var result = new List<SerialiazedObject>(count);
                var idx = 0;

                foreach (var item in data.OrderBy(x => x.Key))
                {
                    int rnd = data
                        .Where(x => Object.ReferenceEquals(x.Value, item.Value.Random))
                        .FirstOrDefault(defaultKeyVal)
                        .Key;

                    result.Add(new SerialiazedObject()
                    {
                        id = idx,
                        data = item.Value.Data.ToArray(),
                        prevId = idx > 0 ? idx - 1 : -1,
                        nextId = idx < count - 1 ? idx + 1 : -1,
                        random = rnd
                    });

                    idx++;
                }
                return result;
            }
            else return null;
        }

        private ListNode ToListNodeData(IEnumerable<SerialiazedObject> data)
        {
            var list = new Dictionary<int, ListNode>();

            var result = data
                .Select(x=> new { 
                    Key = x.id, 
                    Value = new ListNode() { Data = new string(x.data) }
                })
                .ToDictionary(x=>x.Key, x=>x.Value);

            foreach(var item in data)
            {
                result[item.id].Previous = item.prevId != -1 ? result[item.prevId] : null;
                result[item.id].Next = item.nextId != -1 ? result[item.nextId] : null;
                result[item.id].Random = result[item.random];
            }

            return result.OrderBy(x=>x.Key).FirstOrDefault().Value;
        }


        private async Task<IDictionary<int, ListNode>> ExpandTreeAsync(ListNode head)
        {
            Task<IDictionary<int, ListNode>>[] tasks = new Task<IDictionary<int, ListNode>>[2]
           {
                new Task<IDictionary<int, ListNode>>(()=>DirectionExpandTree(head, Direction.Up)),
                new Task<IDictionary<int, ListNode>>(()=>DirectionExpandTree(head, Direction.Down))
           };

            foreach (var task in tasks) task.Start();

            await Task.WhenAll(tasks);

            var offset = tasks[0].Result.Count - 1;

            IDictionary<int, ListNode> result = tasks[0].Result.Union(tasks[1].Result).ToDictionary(x=>x.Key + offset, x=>x.Value);

            return result;
        }

        private IDictionary<int, ListNode> DirectionExpandTree(ListNode head, Direction direction = Direction.Down)
        {
            var result = new Dictionary<int, ListNode>();
            var step = head;
            var idx = 0;

            while (step != null)
            {
                result.Add(idx, step);
                switch (direction)
                {
                    case Direction.Down:
                        step = step.Next != null ? step.Next : null;
                        idx++;
                        break;
                    case Direction.Up:
                        step = step.Previous != null ? step.Previous : null;
                        idx--;
                        break;
                }
            }

            return result;
        }
    }
}