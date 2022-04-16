namespace UnitRFTest
{
    internal class FooterRenderer : IRenderer
    {
        public string Render(PushingMsg message)
        {
            return $"<b>{message.Footer}</b>";
        }
    }
}