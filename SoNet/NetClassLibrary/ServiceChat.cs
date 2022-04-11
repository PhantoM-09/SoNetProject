using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;

namespace NetClassLibrary
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]     //Для предотвращения создания сервиса/сессии под каждого клиента
    public class ServiceChat : IServiceChat
    {
        Dictionary<string, List<ServiceUser>> chatDictionary = new Dictionary<string, List<ServiceUser>>();

        //Блок методов для поста-----------------------------------------------------------------------------------------------
        public void Connect(string idUser, string idFriend)
        {
            try
            {
                ServiceUser user = new ServiceUser { ID = idUser, operationContext = OperationContext.Current };       //Создание нового юзера

                string friendAndMe = idFriend + idUser;
                string meAndFriend = idUser + idFriend;
                int flagExist = 0;
                foreach (var key in chatDictionary.Keys)
                {
                    //Не зарегистрировал ли нас наш собеседник
                    if (key == friendAndMe)
                    {
                        if (!chatDictionary[key].Any(u => u.ID == idUser))
                        {
                            chatDictionary[key].Add(user);
                        }
                        //Флаг который показывает начат ли уже чат
                        flagExist = 1;
                    }
                    //Не зарегестрировали ли мы чат
                    else if (key == meAndFriend)
                    {
                        if (!chatDictionary[key].Any(u => u.ID == idUser))
                        {
                            chatDictionary[key].Add(user);
                        }
                        //Флаг который показывает начат ли уже чат
                        flagExist = 1;
                    }
                }
                //Если наш собеседник не начинал чат и я до этого его не начинал, то значит я его начну сейчас
                if (flagExist == 0)
                {
                    chatDictionary[meAndFriend] = new List<ServiceUser>();
                    chatDictionary[meAndFriend].Add(user);
                }
            }
            catch(Exception ex)
            {
                using(StreamWriter writer = new StreamWriter("d:/1log.txt"))
                {
                    writer.WriteLine(ex.Message);
                }
            }
            
            
        }

        public void Disconnect(string idUser, string idFriend)
        {

            try
            {
                string friendAndMe = idFriend + idUser;
                string meAndFriend = idUser + idFriend;

                foreach (var key in chatDictionary.Keys)
                {
                    if (key == friendAndMe)
                    {
                        chatDictionary[friendAndMe].Remove(chatDictionary[friendAndMe].FirstOrDefault(i => i.ID == idUser));
                    }
                    else if (key == meAndFriend)
                    {
                        chatDictionary[meAndFriend].Remove(chatDictionary[meAndFriend].FirstOrDefault(i => i.ID == idUser));
                    }
                }

            }
            catch (Exception ex)
            {
                using (StreamWriter writer = new StreamWriter("d:/3log.txt"))
                {
                    writer.WriteLine(ex.Message);
                }
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------

        //Пересылка сообщениями
        public void SendMessageToChat(string message, string idUser, string idFriend)
        {
            try
            {
                string friendAndMe = idFriend + idUser;
                string meAndFriend = idUser + idFriend;

                foreach (var key in chatDictionary.Keys)
                {
                    //Проверка на то, начал ли наш собеседник чат с нами
                    if (key == friendAndMe)
                    {
                        foreach (var user in chatDictionary[friendAndMe])
                        {
                            user?.operationContext.GetCallbackChannel<IServiceChatCallback>().MessageCallBack(message, idUser);
                        }
                    }
                    else if(key == meAndFriend)
                    {
                        foreach (var user in chatDictionary[meAndFriend])
                        {
                            user?.operationContext.GetCallbackChannel<IServiceChatCallback>().MessageCallBack(message, idUser);
                        }
                    }
                }

            }
            catch(Exception ex)
            {
                using (StreamWriter writer = new StreamWriter("d:/2log.txt"))
                {
                    writer.WriteLine(ex.Message);
                }
            }
            

        }
    }
}
