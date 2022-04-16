using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitRFTest
{
    public interface IRenderer 
    { 
        string Render(PushingMsg message); 
    }
    public class PushingMsg // Generating an HTML representation of a message
    {
        public string Header { set; get; }
        public string Body { set; get; }
        public string Footer { set; get; }
    }
    public class MessageRenderer : IRenderer 
    { 
        public IReadOnlyList<IRenderer> SubRenderers { get; } 
        public MessageRenderer() 
        { 
            SubRenderers = new List<IRenderer> 
            { 
                new HeaderRenderer(), 
                new BodyRenderer(), 
                new FooterRenderer() 
            }; 
        } 
        public string Render(PushingMsg message) 
        { 
            return SubRenderers.Select(x => x.Render(message)).Aggregate("", (str1, str2) => str1 + str2); 
        } 
    }
}
