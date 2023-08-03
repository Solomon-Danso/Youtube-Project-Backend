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

        [HttpPost("SearchVideo")]
        public async Task<IActionResult> SearchVideo(string searchTerm){
            var searchResult = context.VideoModels .ToList().Where(v=>v.Title != null && v.Title.Contains(searchTerm,StringComparison.OrdinalIgnoreCase)).ToList();
            if(searchResult.Count()==0){
                return NotFound("No Result Found");
            }
            return Ok(searchResult);
        
        }
    }
}