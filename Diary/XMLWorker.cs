using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Diary
{
    class XMLWorker
    {
        private readonly string documentName = "DiaryData.xml";

        private static XMLWorker instance;

        public static XMLWorker GetInstance()
        {
            if (instance == null)
                instance = new XMLWorker();
            return instance;
        }
        private XMLWorker()
        {
            if (!File.Exists(documentName))
            {
                CreateEmptyDiary();
            }
        }

        private void CreateEmptyDiary()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = false;
            settings.Indent = true;
            settings.NewLineOnAttributes = true;

            var xmlWriter = XmlWriter.Create(documentName, settings);

            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("Diary");
            xmlWriter.WriteEndElement();
            xmlWriter.Close();
        }


        private bool CheckUserExist(string login)
        {
            bool userExist = default;

            var reader = XmlReader.Create(documentName);

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name.Equals("User") && reader.GetAttribute("Login") == login)
                    {
                        userExist = true;
                        break;
                    }
                }
            }

            reader.Close();

            return userExist;
        }

        public IEnumerable<IGrouping<string, XElement>> GetUserData(string login)
        {
            XDocument diary = XDocument.Load(documentName);
            XElement root = diary.Root;

            var userData =
                from user in root.Elements()
                where user.Attribute("Login").Value == login
                from record in user.Elements()
                group record by record.Attribute("Date").Value into records
                orderby records.Key descending
                select records;

            return userData;
        }

        public void RegisterUser(string login, string password)
        {
            if (CheckUserExist(login))
            {
                throw new Exception($"User {login} already exists!");
            }
            else
            {
                var xmldoc = new XmlDocument();
                xmldoc.Load(documentName);

                XmlNode root = xmldoc.DocumentElement;
                XmlNode loginAttr = xmldoc.CreateNode(XmlNodeType.Attribute, "Login", "");                
                XmlNode passwordAttr = xmldoc.CreateNode(XmlNodeType.Attribute, "Password", "");

                loginAttr.Value = login;
                passwordAttr.Value = password;

                XmlNode newUser = xmldoc.CreateElement("User");
                newUser.Attributes.SetNamedItem(loginAttr);
                newUser.Attributes.SetNamedItem(passwordAttr);

                root.AppendChild(newUser);

                xmldoc.Save(documentName);
            }
        }

        public bool SignUp(string login, string password)
        {
            bool signed = default;
            if (!CheckUserExist(login))
            {
                throw new Exception($"User {login} doesn`t exist!");
            }
            else
            {
                var reader = XmlReader.Create(documentName);

                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.Name.Equals("User") 
                            && reader.GetAttribute("Login") == login 
                            && reader.GetAttribute("Password") == password)
                        {
                            signed = true;
                            break;
                        }
                    }
                }
                reader.Close();
            }
            return signed;
        }

        public void AddNote(string date, string login, string text)
        {
            var xmldoc = new XmlDocument();
            xmldoc.Load(documentName);

            XmlNode root = xmldoc.SelectSingleNode($"//User[@Login=\"{login}\"]");
            XmlNode dateAttr = xmldoc.CreateNode(XmlNodeType.Attribute, "Date", "");

            dateAttr.Value = date;

            XmlNode newNote = xmldoc.CreateElement("p");
            newNote.Attributes.SetNamedItem(dateAttr);
            newNote.InnerText = text;

            root.AppendChild(newNote);

            xmldoc.Save(documentName);
        }
    }
}
