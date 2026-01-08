using GPT.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using OpenAI_API;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System;
using OpenAI_API.Chat;
using OpenAI_API.Models;
using System.Collections.Generic;
using System.Text;

namespace GPT.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;
        private OpenAIAPI _api;
        private static List<ChatMessage> chatMessages = new List<ChatMessage>();

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
            var apiKey = _configuration["apiKey"];
            _api = new OpenAIAPI(apiKey);

            Console.WriteLine("Response Content: Went in class");//for testing
        }

        public ActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> ChatGPT(Question question)
        {
            if (!ModelState.IsValid)
            {
                //initialize chatMessages with a system message so bot doesnt answer any questions but python related ones
                question.context = "You are an AI bot that only answers python-related questions. If you are given a non-python related question, you will answer with 'I only respond to python related questions'.";
                chatMessages.Add(new ChatMessage(ChatMessageRole.System, question.context));

                //create a new ChatRequest with the conversation state
                var chatRequest = new ChatRequest
                {
                    Model = Model.ChatGPTTurbo, //model
                    Messages = chatMessages //uses existing conversation history
                };

                //adds input to the conversation
                chatRequest.Messages.Add(new ChatMessage(ChatMessageRole.User, question.message));

                //Openai gives a response
                var result = await _api.Chat.CreateChatCompletionAsync(chatRequest);

                //extract reply
                var reply = result.Choices[0].Message;

                //append reply to the conversation history
                chatMessages.Add(reply);

                //var responseText = new StringBuilder();
                //int sysvar = 0;
                //var responseText = reply.Content;
                question.response = reply.Content;

                /*foreach (ChatMessage msg in chatMessages)
                {
                    if (sysvar > 0)
                    {
                        responseText.AppendLine($"{msg.Role}: \n {msg.Content}");
                    }
                    sysvar += 1;
                }*/

                //error checking
                Console.WriteLine($"One     New");
                foreach (ChatMessage msg in chatMessages)
                {
                    Console.WriteLine($"{msg.Role}: {msg.Content}");
                }
                //return response
                return Content(question.response.ToString());
            }
            return View("Index", question);
        }
    }
}
