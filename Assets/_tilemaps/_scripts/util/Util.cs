using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;
using Util;

namespace Util
{
    //TODO figure out if this even has to be a monobehaviour
    public class Util
    {

        public const string logDelim = "********************************";
        //public const string combatLogHeader = "Round,Attacker,Defender,AttackMenStart,AttackGunStart,AttackTankStart,DefendMenStart,DefendGunStart,DefendTankStart,TankHits,GunHits,InfHits,MenDisrupt,MenRetreat,MenDamaged,MenKill,GunDisrupt,GunRetreat,GunDamaged,GunKill,TankDisrupt,TankRetreat,TankDamaged,TankKill";
        public static StringBuilder combatLogMessage = new StringBuilder();     //this stringbuilder will be called throughout the combat routine to dump data out to a csv log file

        /// <summary>
        /// Come back and do this later to encapsulate all the log writing to be debug only
        /// </summary>
        public static void WriteDebugLog(string logMessage, int logLevel = 4)
        {
            //It's not the end of the world if we can't debug log
            try
            {
                //Using this VS GameManager.instance because in the editor this does not work right.
                var gameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
                if (gameManager.debug && gameManager.LogLevel >= logLevel)
                {
                    Debug.Log(logMessage);
                }
            }
            catch
            {

            }
        }

        public static void WritetoDumpLog(string csv, int logLevel = 4)
        {
            var gameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
            if (gameManager.debug && gameManager.LogLevel >= logLevel)
            {
                DateTime time = DateTime.Now;
                string format = "MM-dd-yy-HH-mm";
                var filePath = string.Format("{0}\\WarGame-Dump-{1}.log", Application.persistentDataPath, time.ToString(format));

                //Not sure if stringbuilder is the right optimization but well cross that bridge when we need to.
                var csvSB = new StringBuilder();
                csvSB.AppendLine(csv);
                DataDump.WriteToCSV(filePath, csvSB);
            }
        }

        //public static void WriteCombatLog(string csv)
        //{
        //    var gameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
        //    if (gameManager.debug && gameManager.combatdebug)
        //    {
        //        DateTime time = DateTime.Now;
        //        string format = "MM-dd-yy-HH-mm";
        //        var filePath = string.Format("{0}\\WarGame-combatlog-{1}.csv", Application.persistentDataPath, time.ToString(format));

        //        //Not sure if stringbuilder is the right optimization but well cross that bridge when we need to.
        //        var csvSB = new StringBuilder();
        //        csvSB.AppendLine(csv);

        //        DataDump.WriteToCSV(filePath, csvSB);
        //    }

        //}

        public static void QuitTheGame()
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        public static Mesh SpriteToMesh(Sprite sprite)
        {
            Mesh mesh = new Mesh();
            mesh.vertices = Array.ConvertAll(sprite.vertices, i => (Vector3)i);
            mesh.uv = sprite.uv;
            mesh.triangles = Array.ConvertAll(sprite.triangles, i => (int)i);

            return mesh;
        }

        /// <summary>
        /// Converts to list and back.
        /// Pass in an array and a index position to remove at
        /// </summary>
        /// <returns>The to list and back.</returns>
        /// <param name="array">Array.</param>
        /// <param name="removeAt">Remove at.</param>
        public static string[] ConvertStringToListAndBack(string[] array, int removeAt)
        {
            var foos = new List<string>(array);
            foos.RemoveAt(removeAt);
            return foos.ToArray();
        }

    } 
}
