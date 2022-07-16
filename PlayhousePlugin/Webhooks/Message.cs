namespace PlayhousePlugin.Webhooks
{
    public class Message
    {
        public Message(string content)
        {
            //username = PlayhousePlugin.Singleton.Config.Username;
            //avatar_url = PlayhousePlugin.Singleton.Config.AvatarUrl;
            this.content = content;
        }
            
        //public string username { get; }
        //public  string avatar_url { get; }
        public  string content { get; }
    }
}