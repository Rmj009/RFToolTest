namespace UnitRFTest
{
    internal class HeaderRenderer : IRenderer
    {
        public string Render(PushingMsg message)
        {
            return $"<b>{message.Header}</b>";
        }
    }
}