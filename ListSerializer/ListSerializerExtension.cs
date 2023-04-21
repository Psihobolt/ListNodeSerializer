using SerializerTests.Nodes;
using System.Collections.Concurrent;
using System.Text;

namespace ListSerializer
{
    public static class ListNodeExtension
    {

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
        /// Returns the dictionary of elements in a <see cref="ListNode"/>, starting from the passed element and moving through the list in the specified direction
        /// </summary>
        public static IEnumerable<SerialiazedObject> ToSerializeData(this ListNode head)
        {
            var visited = new Dictionary<int, ListNode>();
            var result = new List<SerialiazedObject>();
            var defaultKeyVal = new KeyValuePair<int, ListNode>(-1, null);

            var node = head;

            if (node.Previous != null)
            {
                while (node.Previous != null) node = node.Previous;
            }

            var idx = 0;
            while (node != null)
            {
                result.Add(new SerialiazedObject()
                {
                    id = idx,
                    data = node.Data.ToArray(),
                    prevId = idx > 0 ? idx - 1 : -1,
                    nextId = node.Next != null ? idx + 1 : -1,
                    random = -1
                });

                visited.Add(idx, node);
                idx++;

                // Guard for cycle link
                if (Object.ReferenceEquals(node.Next, visited.First().Value))
                {
                    result.Last().nextId = 0;
                    node = null;
                }
                else
                {
                    node = node.Next;
                }
            }

            head = visited.First().Value;
            idx = 0;

            foreach (var item in result)
            {
                var rnd = visited
                    .Where(x => Object.ReferenceEquals(x.Value, head.Random))
                    .FirstOrDefault(defaultKeyVal)
                    .Key;

                head = head.Next;
                item.random = rnd;
            }

            visited.Clear();
            return result;
        }
    }

    public static class ListSerializedObjectExtension
    {
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
