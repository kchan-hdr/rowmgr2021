using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ROWM
{
    public class SwaggerIgnores: IActionModelConvention
    {
        public void Apply(ActionModel m)
        {
            if (Controller.ControllerName.Equals("Pwa"))
                m.ApiExplorer.IsVisible = false;
        }
    }
}
