using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperConsoleDemo.BaseModels
{
    public class Bill
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Color { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }

        public int CustomerId { get; set; }
    }
}
