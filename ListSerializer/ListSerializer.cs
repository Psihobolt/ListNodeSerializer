
using SerializerTests.Interfaces;
using SerializerTests.Nodes;
using System.Runtime.InteropServices;
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
        public async Task<ListNode> DeepCopy(ListNode head)
        {
            var list = await head.ExpandTreeAsync();
            return list.ToSerializeData().ToListNodeData();
        }

        public async Task<ListNode> Deserialize(Stream s)
        {
            s.Position = 0;

            JsonSerializerOptions jsonOption = new() { IncludeFields = true };

            var data = await JsonSerializer.DeserializeAsync<List<SerialiazedObject>>(s, jsonOption);

            return data?.ToListNodeData();
        }

        public async Task Serialize(ListNode head, Stream s)
        {
            s.Position = 0;

            var node = await head.ExpandTreeAsync();
            var data = node.ToSerializeData();

            JsonSerializerOptions jsonOption = new() { IncludeFields = true };
            await JsonSerializer.SerializeAsync(s, data, data.GetType(), jsonOption);
            s.Flush();
        }
    }
}