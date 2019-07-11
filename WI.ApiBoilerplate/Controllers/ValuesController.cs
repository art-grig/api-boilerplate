using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using WI.Foundation.ViewModels;

namespace WI.ApiBoilerplate.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
	public class ValuesController : ApiController
    {
        // GET api/values
        [HttpGet]
        public ActionResult<ResponseVm<string[]>> Get()
        {
            return RespInnerTyped(new ResponseVm<string[]>(new string[] { "value1", "value2" }));
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<ResponseVm<string>> Get([FromRoute]int id)
        {
            return RespInnerTyped(new ResponseVm<string>("value"));
        }

        // POST api/values
        [HttpPost]
        public void Post(string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put([FromRoute]int id, string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
