using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;

namespace WebSheepshead.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public List<Helpers.ChatMessage> Get(int id)
        {
            return Helpers.ChatSession.messages.Where(x=> ((x.MessageNumber>id) && (!((x.MessageSender!=null)&&(x.MessageText.StartsWith("/"))))
            &&((x.MessageRecipient == null)||(x.MessageRecipient.Name==User.Identity.Name)))).ToList();
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
            if (value == "/RESET")
            {
                Helpers.ChatSession.ResetGame();
            }
            else if (value.StartsWith("/KICK"))
            {
                var playerToKick = value.Split(' ')[1];
                
            }
            else
            {
                Helpers.ChatSession.messages.Add(new Helpers.ChatMessage
                {
                    MessageNumber = Helpers.ChatSession.messages.Count,
                    MessageText = value,
                    MessageSender = User.Identity
                });
            }
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}