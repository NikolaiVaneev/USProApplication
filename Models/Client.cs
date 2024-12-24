using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USProApplication.Models;

public class Client
{
    public Guid? Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public string Inn { get; set; } = string.Empty;
}
