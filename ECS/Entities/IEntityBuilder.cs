using ECS.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS.Entities
{
    public interface IEntityBuilder
    {
        Entity Build(Scene scene);
    }
}
