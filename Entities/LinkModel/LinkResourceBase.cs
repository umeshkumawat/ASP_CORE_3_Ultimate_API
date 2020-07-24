using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Entities.LinkModel
{
    public class LinkResourceBase
    {
        public LinkResourceBase()
        { }

        public List<Link> Links { get; set; } = new List<Link>();
    }
}
