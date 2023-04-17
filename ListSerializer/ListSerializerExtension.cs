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

        /// <summary>
        /// Returns a string with the value in the node and the value of the previous and next node
        /// </summary>
        public static string ToFullString(this ListNode node)
        { 
            return String.Join("_",
                node.Data,
                node.Previous == null ? "Root" : node.Previous.Data,
                node.Next == null ? "End" : node.Next.Data);
        }

        /// <summary>
        /// Returns the dictionary of all elements in a linked list, starting from the list root
        /// </summary>
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

        /// <summary>
        /// Returns the dictionary of elements in a <see cref="ListNode"/>, starting from the passed element and moving through the list in the specified direction
        /// </summary>
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
        /// <summary>
        /// Returns a list of <see cref="SerialiazedObject"/> that are ready for serialization/deserialization
        /// </summary>
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

        /// <summary>
        /// Returns the root of a linked list generated from a list ready for serialization/deserialization
        /// </summary>
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
