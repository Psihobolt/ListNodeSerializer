using SerializerTests.Nodes;

namespace ListSerializer.App
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var head = new ListNode() { Data = "Head", Previous = null };
            var node1 = new ListNode() { Data = "Node 1", Previous = head };
            var node2 = new ListNode() { Data = "Node 2", Previous = node1 };
            var node3 = new ListNode() { Data = "Node 3", Previous = node2 };
            var node4 = new ListNode() { Data = "Node 4", Previous = node3 };
            var node5 = new ListNode() { Data = "Node 5", Previous = node4 };
            var node6 = new ListNode() { Data = "Node 6", Previous = node5 };
            var node7 = new ListNode() { Data = "Node 7", Previous = node6 };
            var node8 = new ListNode() { Data = "Node 8", Previous = node7, Next = null };

            head.Next = node1;
            node1.Next = node2;
            node2.Next = node3;
            node3.Next = node4;
            node4.Next = node5;
            node5.Next = node6;
            node6.Next = node7;
            node7.Next = node8;

            head.Random = node7;
            node1.Random = node5;
            node2.Random = node6;
            node3.Random = node2;
            node4.Random = head;
            node5.Random = node8;
            node6.Random = node1;
            node7.Random = node3;
            node8.Random = node4;

            var serilizer = new ListSerializer();
            using (var buffer = new MemoryStream())
            {
                buffer.Position = 0;
                var a = serilizer.Serialize(head, buffer);
                var b = serilizer.Deserialize(buffer);
            }
        }
    }
}