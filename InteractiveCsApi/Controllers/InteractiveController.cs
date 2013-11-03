using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using InteractiveCsApi.Models;
using Roslyn.Compilers;
using Roslyn.Scripting;
using Roslyn.Scripting.CSharp;

namespace InteractiveCsApi.Controllers
{
    public class InteractiveController : ApiController
    {
        public HttpResponseMessage Post([FromBody] Code code)
        {
            var scriptEngine = new ScriptEngine();
            Session session = scriptEngine.CreateSession();
            session.AddReference("System");
            session.AddReference("System.Core");
            if (code.Buffer != null)
            {
                code.Buffer = code.Buffer.Where(x => x.EndsWith(";")).ToList();
                code.Buffer.Add(code.CurrentCode);
            }
            else
                code.Buffer = new List<string> { code.CurrentCode };
            var toExecute = string.Join(" ", code.Buffer);
            try
            {
                var result = session.Execute(toExecute);
                return Request.CreateResponse(HttpStatusCode.OK,
                    result == null
                        ? new Code { Success = true, Message = string.Empty, Buffer = code.Buffer }
                        : new Code { Success = true, Message = result.ToString(), Buffer = code.Buffer });
            }
            catch (CompilationErrorException errorException)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new Code { Success = false, Message = errorException.Message, Buffer = code.Buffer });
            }

        }
    }
}
