
using SerializerTests.Interfaces;
using SerializerTests.Nodes;
using System.Net.Http.Json;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

namespace ListSerializer
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public record SerialiazedObject
    {
        public int id;
        public int nextId;
        public int prevId;
        public int random;
        public char[] data;
    }

    public class ListSerializer : IListSerializer
    {
        public Task<ListNode> DeepCopy(ListNode head)
        {
            return Task.FromResult(head.ToSerializeData().ToListNodeData());
        }

        public Task<ListNode> Deserialize(Stream s)
        {
            s.Position = 0;
            List<SerialiazedObject> data = new List<SerialiazedObject>();

            using (var reader = new BinaryReader(s, Encoding.UTF8, false))
            {
                while (s.Position < s.Length)
                {
                    var obj = new SerialiazedObject() {
                        id = reader.ReadInt32(),
                        nextId = reader.ReadInt32(),
                        prevId = reader.ReadInt32(),
                        random = reader.ReadInt32()
                    };

                    var len = reader.ReadInt32();
                    obj.data = reader.ReadChars(len);
                    data.Add(obj);
                }
            }

            return Task.FromResult(data.ToListNodeData());
        }

        public Task Serialize(ListNode head, Stream s)
        {
            s.Position = 0;

            var data = head.ToSerializeData();

            using (var writer = new BinaryWriter(s, Encoding.UTF8, true))
            {
                foreach (var item in data)
                {
                    writer.Write(item.id);
                    writer.Write(item.nextId);
                    writer.Write(item.prevId);
                    writer.Write(item.random);
                    writer.Write(item.data.Length);
                    writer.Write(item.data);
                    s.Flush();
                }
            }

            return Task.CompletedTask;
        }
    }
}