using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using YouTube_Backend.Data;

namespace YouTube_Backend.Controllers
{
    [ApiController]
    [Route("api/AllOtherFeatures")]
    public class AllOtherFeaturesController : ControllerBase
    {
        private readonly DataContext context;
        public AllOtherFeaturesController(DataContext ctx){
            context = ctx;
        }
        
    }
}