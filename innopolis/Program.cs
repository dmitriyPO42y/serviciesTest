using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace innopolis
{
    class Program
    {
        static string url = "http://10.240.18.241:5000/";
        const string ERROR = "error";


        static void Main(string[] args)
        {
            #region Проверка ИНН
            string generateINNUrl = "inn/generate";
            Console.WriteLine("Проверка метода, генерирующего ИНН (без входного параметра)");
            viewResponse(JsonConvert.DeserializeObject<Dictionary<string, string>>(GET(url + generateINNUrl, "")), generateINNUrl);
            Console.WriteLine("Проверка метода, генерирующего ИНН (Для юридического лица)");
            viewResponse(JsonConvert.DeserializeObject<Dictionary<string, string>>(GET(url + generateINNUrl, "type=юр")), generateINNUrl);
            Console.WriteLine("Проверка метода, генерирующего ИНН (Для физического лица)");
            viewResponse(JsonConvert.DeserializeObject<Dictionary<string, string>>(GET(url + generateINNUrl, "type=физ")), generateINNUrl);
            #endregion
            
            #region Проверка СНИЛС
            string generateSNILSUrl = "snils/generate";
            Console.WriteLine("Проверка метода, генерирующего СНИЛС (Генерируется без входных параметров)");
            viewResponse(JsonConvert.DeserializeObject<Dictionary<string, string>>(GET(url + generateSNILSUrl, "")), generateSNILSUrl);
            Console.WriteLine("Проверка метода, генерирующего СНИЛС (С ошибочным адресом)");
            viewResponse(JsonConvert.DeserializeObject<Dictionary<string, string>>(GET(url + generateSNILSUrl + "gen", "")), generateSNILSUrl);
            #endregion

            #region Проверка даты
            string generateDateUrl = "date/generate";
            Console.WriteLine("Проверка метода, генерирующего дату (со сдвигом на 5 дней вперед)");
            viewResponse(JsonConvert.DeserializeObject<Dictionary<string, string>>(GET(url + generateDateUrl, "mask=%d.%m.%Y&target=now&offset=5")), generateDateUrl);
            Console.WriteLine("Проверка метода, генерирующего дату (со сдвигом на 5 дней назад)");
            viewResponse(JsonConvert.DeserializeObject<Dictionary<string, string>>(GET(url + generateDateUrl, "mask=%d.%m.%Y&target=now&offset=-5")), generateDateUrl);
            #endregion

            #region Проверка генератора номеровбанковских карт
            string generateBankCardNumberUrl = "bankcardnumber/generate";
            Console.WriteLine("Проверка метода, генерирующего номер карты (без входных параметров)");
            viewResponse(JsonConvert.DeserializeObject<Dictionary<string, string>>(GET(url + generateBankCardNumberUrl, "")), generateBankCardNumberUrl);
            #endregion

            #region Проверка номера телефона
            string generatePhoneNumberUrl = "phonenumber/generate";
            Console.WriteLine("Проверка метода, генерирующего номер телефона (без входных параметров)");
            viewResponse(JsonConvert.DeserializeObject<Dictionary<string, string>>(GET(url + generatePhoneNumberUrl, "code_country=7&code_city=846")), generatePhoneNumberUrl);
            #endregion

            #region Проверка генерации имени, фамилии и отчества
            string generateUserUrl = "user/";
            Console.WriteLine("Проверка метода, генерирующего имя.");
            string genName = "gen_name/";
            viewResponse(JsonConvert.DeserializeObject<Dictionary<string, string>>(GET(url + generateUserUrl + genName, "sex=man&lang=rus")), genName);
            string genSecondName = "gen_second_name/";
            viewResponse(JsonConvert.DeserializeObject<Dictionary<string, string>>(GET(url + generateUserUrl + genSecondName, "sex=man&lang=eng")), genSecondName);
            string genSurName = "gen_surname/";
            viewResponse(JsonConvert.DeserializeObject<Dictionary<string, string>>(GET(url + generateUserUrl + genSurName, "sex=women&lang=rus")), genSurName);
            string genFIO = "gen_fio/";
            viewResponse(JsonConvert.DeserializeObject<Dictionary<string, string>>(GET(url + generateUserUrl + genFIO, "sex=women&lang=rus")), genFIO);
            #endregion

            #region Проверка КПП
            Console.WriteLine("Проверка метода, генерирующего КПП.");
            string genKPP = "kpp/generate";
            viewResponse(JsonConvert.DeserializeObject<Dictionary<string, string>>(GET(url + genKPP, "")), genKPP);
            #endregion

            #region Проверка генератора автомобильных номеров
            Console.WriteLine("Проверка метода, генерирующего государственные регистрационные знаки ТС.");
            string geтTsNumber = "autonumber/generate";
            viewResponse(JsonConvert.DeserializeObject<Dictionary<string, string>>(GET(url + geтTsNumber, "type_num=мотоцикл")), geтTsNumber);
            viewResponse(JsonConvert.DeserializeObject<Dictionary<string, string>>(GET(url + geтTsNumber, "type_num=легковой")), geтTsNumber);
            viewResponse(JsonConvert.DeserializeObject<Dictionary<string, string>>(GET(url + geтTsNumber, "type_num=прицеп")), geтTsNumber);
            #endregion

            Console.ReadKey();
        }

        private static void viewResponse(Dictionary<string, string> dictionary, string url)
        {
            if (dictionary == null)
            {
                return;
            }

            Console.WriteLine();
            Console.WriteLine(string.Format("Проверяется метод: {0}", url));
            foreach (var item in dictionary)
            {
                if (string.IsNullOrEmpty(item.Value))
                {
                    Console.WriteLine("Получено пустое значение.");
                    return;
                }

                if (item.Key == ERROR)
                {
                    Console.WriteLine("Ошибка при выполнении запроса. Не верный входной параметр.");
                    return;
                }

                Console.WriteLine(string.Format("Получено значение {0} = {1}.", item.Key, item.Value));
            }
        }

        private static string GET(string Url, string Data)
        {
            WebRequest req = WebRequest.Create(Url + "?" + Data);
            try
            {
                HttpWebResponse response = (HttpWebResponse)req.GetResponse();
                using (Stream stream = response.GetResponseStream())
                using (StreamReader sr = new StreamReader(stream))
                {
                    string Out = sr.ReadToEnd();
                    return Out;
                }
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    Console.WriteLine("Ошибка! не правильно указан URL. Код ошибки: {0}", ((HttpWebResponse)e.Response).StatusCode);
                }
            }
            return string.Empty;
        }
    }
}
