namespace UnitRFTest
{
    internal class BodyRenderer : IRenderer
    {
        public string Render(PushingMsg message)
        {
            return $"<b>{message.Body}</b>";
        }
    }
}