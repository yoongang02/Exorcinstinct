using System.Collections.Generic;
using UnityEngine;

namespace OpenAI
{
    public class GptManager : MonoBehaviour
    {
        private OpenAIApi openai = new OpenAIApi();
        private List<ChatMessage> contexts = new List<ChatMessage>(); // 대화 맥락

        private string prompt = "";
    }
}
