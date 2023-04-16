using SerializerTests.Nodes;
using System.Collections.Concurrent;

namespace ListSerializer
{
    public static class ListNodeExtension
    {
        private enum Direction
        {
            Down,
            Up
        }

        public static string ToFullString(this ListNode node)
        {
            return String.Join("_",
                node.Data,
                node.Previous == null ? "Root" : node.Previous.Data,
                node.Next == null ? "End" : node.Next.Data);
        }

        public static async Task<IDictionary<int, ListNode>> ExpandTreeAsync(this ListNode head)
        {
            var list = new ConcurrentDictionary<int, ListNode>();
            Task[] tasks = new Task[2]
            {
                new Task(async() => await head.StepExpandTree(list, direction: Direction.Up)),
                new Task(async() => await head.StepExpandTree(list))
            };

            foreach (var task in tasks) task.Start();
            await Task.WhenAll(tasks);

            var offset = list.Where(x => x.Key < 0).Count();

            return list.OrderBy(x=>x.Key).ToDictionary(x => x.Key + offset, x => x.Value);
        }

        public static IDictionary<int, ListNode> ExpandTree(this ListNode head)
        {
            Task<IDictionary<int, ListNode>>[] tasks = new Task<IDictionary<int, ListNode>>[2]
            {
                new Task<IDictionary<int, ListNode>>(() => head.DirectionExpandTree(Direction.Up)),
                new Task<IDictionary<int, ListNode>>(() => head.DirectionExpandTree())
            };

            foreach (var task in tasks) task.Start();
            Task.WhenAll(tasks);

            var offset = tasks[0].Result.Count();

            return tasks[0].Result.Union(tasks[1].Result).OrderBy(x=>x.Key).ToDictionary(x => x.Key + offset, x => x.Value);
        }

        private static IDictionary<int, ListNode> DirectionExpandTree(this ListNode head, Direction direction = Direction.Down)
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

        private async static Task StepExpandTree(this ListNode head, ConcurrentDictionary<int, ListNode> dict, int index = 0, Direction direction = Direction.Down)
        {
            dict.TryAdd(index, head);
            switch (direction)
            {
                case Direction.Down:
                    if (head.Next != null)
                    {
                        await head.Next.StepExpandTree(dict, index + 1, direction);
                    }
                    break;
                case Direction.Up:
                    if (head.Previous != null)
                    {
                        await head.Previous.StepExpandTree(dict, index - 1, direction);
                    }
                    break;
            }
        }
    }
    public static class ListSerializerExtension
    {
        public static IEnumerable<SerialiazedObject> ToSerializeData(this IDictionary<int, ListNode> data)
        {
            var defaultKeyVal = new KeyValuePair<int, ListNode>(-1, null);

            var count = data.Count;
            var result = new List<SerialiazedObject>(count);
            var idx = 0;

            foreach (var item in data)
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

        public static ListNode ToListNodeData(this IEnumerable<SerialiazedObject> data)
        {
            var result = data
                .Select(x => new {
                    Key = x.id,
                    Value = new ListNode() { Data = new string(x.data) }
                })
                .ToDictionary(x => x.Key, x => x.Value);

            foreach (var item in data)
            {
                result[item.id].Previous = item.prevId != -1 ? result[item.prevId] : null;
                result[item.id].Next = item.nextId != -1 ? result[item.nextId] : null;
                result[item.id].Random = result[item.random];
            }

            return result.FirstOrDefault().Value;
        }
    }
}
