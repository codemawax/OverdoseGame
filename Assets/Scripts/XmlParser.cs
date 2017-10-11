using UnityEngine;
using System.Collections;
using System.Xml;
using System.Collections.Generic;

public class XmlParser : MonoBehaviour {

    static public void GetLevel(int levelId, ref GameController.Level levelToLoad)
    {
        Debug.Log("GET LEVEL");
        XmlDocument xmlDoc = LoadXMLFromAsset("Level" + levelId.ToString() + ".xml");
        ParseLevel(xmlDoc, ref levelToLoad);
    }
    

    static private XmlDocument LoadXMLFromAsset(string fileName)
    {
        XmlDocument xmlDoc = new XmlDocument();
        if (System.IO.File.Exists(GetPath(fileName)))
        {
            xmlDoc.LoadXml(System.IO.File.ReadAllText(GetPath(fileName)));
        }
        else
        {
            TextAsset textXml = (TextAsset)Resources.Load(fileName, typeof(TextAsset));
            xmlDoc.LoadXml(textXml.text);
        }
        return xmlDoc;
    }


    static private string GetPath(string fileName)
    {
        #if UNITY_EDITOR
        return Application.dataPath + "/Resources/" + fileName;
        #endif
    }


    static private void ParseLevel(XmlDocument xmlDoc, ref GameController.Level levelToLoad)
    {

        Debug.Log("PARSE LEVEL");

        XmlNodeList levelInfo = xmlDoc.GetElementsByTagName("levelInfo");
        XmlNodeList hazardList = xmlDoc.GetElementsByTagName("hazard");

        levelToLoad.levelArray = new ArrayList();

        foreach (XmlElement infoNode in levelInfo)
        {
            XmlNodeList infoList = infoNode.ChildNodes;

            foreach (XmlElement info in infoList)
            {
                switch (info.Name)
                {
                    case "id":
                        levelToLoad.id = int.Parse(info.InnerText);
                        break;

                    case "name":
                        Debug.Log(info.InnerText);
                        levelToLoad.name = info.InnerText;
                        break;

                    case "bpm":
                        levelToLoad.bpm = float.Parse(info.InnerText);
                        break;

                    case "offset":
                        levelToLoad.offset = float.Parse(info.InnerText);
                        break;

                    case "audioFile":
                        levelToLoad.audioFile = info.InnerText;
                        break;
                }
            }
        }

        foreach (XmlElement hazard in hazardList)
        {
            XmlNodeList objectList = hazard.ChildNodes;

            GameController.Hazard tempHazard = new GameController.Hazard();
            tempHazard.hazardObjects = new ArrayList();

            tempHazard.bar = float.Parse(hazard.Attributes["bar"].Value);

            foreach (XmlElement hazardObjectNode in objectList)
            {
                XmlNodeList hazardObject = hazardObjectNode.ChildNodes;

                GameController.HazardObject tempHazardObject = new GameController.HazardObject();

                foreach (XmlElement objectElement in hazardObject)
                {
                    switch (objectElement.Name)
                    {
                        case "objectName":
                            tempHazardObject.objectName = objectElement.InnerText;
                            break;

                        case "spawnSide":
                            tempHazardObject.spawnSide = objectElement.InnerText;
                            break;

                        case "spawnPosition":
                            tempHazardObject.spawnPosition = float.Parse(objectElement.InnerText);
                            break;
                    }
                }
                tempHazard.hazardObjects.Add(tempHazardObject);
            }
            levelToLoad.levelArray.Add(tempHazard);
        }
    }






}
