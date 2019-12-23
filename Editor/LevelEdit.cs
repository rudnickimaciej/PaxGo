using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;

namespace LevelEditor
{
    public class LevelEdit : EditorWindow
    {
        public GUISkin skin;
        #region textures
        [Header("Textures")]

        public Texture red;
        public Texture yellow;
        public Texture blue;
        public Texture green;
        public Texture emptyKind;

        public Texture arrowUp;
        public Texture arrowDown;
        public Texture arrowLeft;
        public Texture arrowRight;
        public Texture emptyDirection;
        public Texture logo;
        #endregion
        List<IGUI> guis = new List<IGUI>();
        public int width = 8;
        public int height = 8;
        GUIBoard GUIBoard;
        BlockEditor BlockEditor;
        string fileName = "file_name.json";
        string pathToJson;
        string pathToSave;
        string openedJson;
        void OnEnable()
        {

            BlockEditor = new BlockEditor();
            BlockEditor.Init();

            GUIBoard = new GUIBoard(width, height);
            GUIBoard.InitBoard(BlockEditor);
        }

        [MenuItem("Window/LevelEditor %l")]
        public static void ShowWindow()
        {
            LevelEdit w = (LevelEdit)EditorWindow.GetWindowWithRect(typeof(LevelEdit), new Rect(0, 0, 780, 550), false, "LEVEL EDITOR");
            w.maxSize = new Vector2(780, 550);
            w.minSize = new Vector2(10, 10);
        }

        void OnGUI()
        {
            GUI.skin = skin;
            // GUILayout.BeginArea(new Rect(20, 10, 540, 20), EditorStyles.helpBox);
            // GUILayout.BeginHorizontal();

            // width = EditorGUILayout.IntField("Board width: ", width, GUILayout.MaxWidth(220f));
            // height = EditorGUILayout.IntField("Board height: ", height, GUILayout.MaxWidth(220f));

            // GUILayout.EndHorizontal();
            // GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(20, 20, 800, 800));

            GUIBoard.OnGUI();

            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(550, 20, 200, 170), EditorStyles.helpBox);

            BlockEditor.OnGUI();

            GUILayout.EndArea();


            GUILayout.BeginArea(new Rect(550, 230, 200, 50), EditorStyles.helpBox);
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            fileName = GUILayout.TextField(fileName);
            if (GUILayout.Button("SAVE", "save_btn"))
            {
                if (pathToSave != null)
                    Save();
            }
            GUILayout.Space(10f);

            GUILayout.EndVertical();
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(550, 290, 200, 100), EditorStyles.helpBox);
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            GUILayout.Label("Directory to save JSON:", "cent");
            GUILayout.Space(10f);
            GUILayout.Label(pathToSave, "left");
            GUILayout.Space(10f);
            if (GUILayout.Button("CHANGE", "save_btn"))
            {
                pathToSave = EditorUtility.OpenFolderPanel("TITLE", "Levels Layouts", "");

            }
            GUILayout.EndVertical();
            GUILayout.EndArea();


            GUILayout.BeginArea(new Rect(550, 400, 200, 100), EditorStyles.helpBox);
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            GUILayout.Label("Opened Json:", "cent");
            GUILayout.Space(10f);
            GUILayout.Label(openedJson, "left");
            GUILayout.Space(10f);
            if (GUILayout.Button("OPEN", "save_btn"))
            {
                openedJson = EditorUtility.OpenFilePanel("SELECT JSON FILE", Application.dataPath + "/Levels Layouts", "json");
                OpenJson();

            }
            GUILayout.EndVertical();
            GUILayout.EndArea();

            GUI.Box(new Rect(700, 500, 65, 65), logo);
        }

        void Save()
        {
            Blocks blocks = new Blocks(GUIBoard.ReturnBlocks());
            string json = JsonUtility.ToJson(blocks);

            if (File.Exists(pathToSave + "/" + fileName))
            {
                File.Delete(pathToSave + "/" + fileName);
                File.Create(pathToSave + "/" + fileName).Dispose();
                File.WriteAllText(pathToSave + "/" + fileName, json);
            }

            else
            {
                File.Create(pathToSave + "/" + fileName).Dispose();
                File.WriteAllText(pathToSave + "/" + fileName, json);
            }
        }

        void OpenJson()
        {

            foreach (GUIEntity e in GUIBoard.entities)
            {
                e.SetDefault();
            }
            Blocks blocks = JsonUtility.FromJson<Blocks>(File.ReadAllText(openedJson));



            foreach (Block b in blocks.blocks)
            {
                GUIBoard.entities[b.x - 1, b.y - 1].Kind = b.kind;
                GUIBoard.entities[b.x - 1, b.y - 1].Dir = b.direction;
            }
        }
    }
    public class GUIBoard : IGUI
    {

        public GUIEntity[,] entities;
        int width;
        int height;

        public GUIBoard(int width, int height)
        {
            this.width = width;
            this.height = height;
        }
        public List<Block> ReturnBlocks()
        {
            List<Block> blocks = new List<Block>();
            foreach (GUIEntity e in entities)
            {
                if (e.Kind != null && e.Dir != null)
                {
                    Block b = new Block((Direction)e.Dir, (Kind)e.Kind, (int)e.GetCoords().x, (int)e.GetCoords().y);
                    b.slug = null;

                    blocks.Add(b);
                }
            }
            return blocks;
        }
        public void InitBoard(BlockEditor blockEditor)
        {
            entities = new GUIEntity[width, height];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    GUIEntity e = new GUIEntity();
                    e.SetDefault();
                    e.EntityClicked += blockEditor.SetEntity;


                    entities[i, j] = e;
                    e.x = i;
                    e.y = j;

                }
            }

        }
        public void OnGUI()
        {
            int x = 0;
            int y = 0;
            for (int j = height - 1; j >= 0; j--)
            {
                for (int i = 0; i < width; i++)
                {

                    entities[i, j].SetPos(x * 60, y * 60);
                    entities[i, j].OnGUI();
                    x++;
                }
                y++;
                x = 0;
            }

        }



    }


    public class GUIEntity : IGUI
    {

        public int x;
        public int y;

        public int xPos;
        public int yPos;

        Direction? dir;
        public Direction? Dir
        {
            set
            {
                dir = value;
                DirectionTexture = Resources.Load<Texture>("Editor/Textures/" + dir.ToString());
            }
            get { return dir; }
        }
        public Texture DirectionTexture;



        Kind? kind;
        public Kind? Kind
        {
            set
            {
                kind = value;
                KindTexture = Resources.Load<Texture>("Editor/Textures/" + kind.ToString());
            }

            get { return kind; }

        }


        public Texture KindTexture;

        public BlockEditor BlockEditor;

        public delegate void OnEntityClicked(GUIEntity entity, Event e);
        public event OnEntityClicked EntityClicked;


        public void SetPos(int xPos, int yPos)
        {
            this.xPos = xPos;
            this.yPos = yPos;
        }

        public Vector2 GetCoords()
        {
            return new Vector2(x + 1, y + 1);
        }

        public void SetDefault()
        {
            kind = null;
            KindTexture = Resources.Load<Texture>("Editor/Textures/empty");
            dir = null;
            DirectionTexture = null;
        }
        public void OnGUI()
        {
            GUILayout.BeginArea(new Rect(xPos, yPos, 55, 55), KindTexture);
            if (GUI.Button(new Rect(0, 0, 55, 55), DirectionTexture))
            {

                EntityClicked(this, Event.current);

            }
            GUILayout.EndArea();
        }
    }
    public class BlockEditor : IGUI
    {
        public GUIEntity CurrentlyEditedEntity;

        public Kind kind = Kind.blue;
        public Direction dir = Direction.North;

        List<KindOption> kindOptions;

        List<DirectionOption> dirOptions;



        public void Init()
        {
            kindOptions = new List<KindOption>();

            kindOptions.Add(new KindOption(Kind.red, 0));
            kindOptions.Add(new KindOption(Kind.green, 50));
            kindOptions.Add(new KindOption(Kind.blue, 100));
            kindOptions.Add(new KindOption(Kind.yellow, 150));

            foreach (KindOption k in kindOptions)
            {
                k.KindChanged += SetKind;
            }

            dirOptions = new List<DirectionOption>();

            dirOptions.Add(new DirectionOption(Direction.North, 0));
            dirOptions.Add(new DirectionOption(Direction.South, 50));
            dirOptions.Add(new DirectionOption(Direction.West, 100));
            dirOptions.Add(new DirectionOption(Direction.East, 150));

            foreach (DirectionOption d in dirOptions)
            {
                d.DirChanged += SetDIrection;

            }
        }
        public void SetEntity(GUIEntity entity, Event e)
        {
            CurrentlyEditedEntity = entity;
            if (Event.current.button == 1)
            {
                entity.SetDefault();
            }
        }

        public void SetKind(Kind kind)
        {
            this.kind = kind;
            CurrentlyEditedEntity.Kind = kind;
        }

        public void SetDIrection(Direction dir)
        {
            this.dir = dir;
            CurrentlyEditedEntity.Dir = dir;
        }
        public void OnGUI()
        {

            GUILayout.BeginHorizontal();

            foreach (KindOption k in kindOptions)
            {
                k.OnGUI();
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            foreach (DirectionOption d in dirOptions)
            {
                d.OnGUI();
            }

            GUILayout.EndHorizontal();
        }
    }

    public class KindOption : IGUI
    {

        Kind kind;
        float posX;
        bool selected = false;

        public delegate void OnKindChanged(Kind kind);
        public event OnKindChanged KindChanged;



        public KindOption(Kind kind, float posX)
        {
            this.kind = kind;
            this.posX = posX;

        }
        public void OnGUI()
        {
            if (GUI.Button(new Rect(posX, 0, 50, 50), Resources.Load<Texture>("Editor/Textures/" + kind.ToString())))
            {
                KindChanged(this.kind);
            }
        }
    }

    public class DirectionOption : IGUI
    {
        Direction dir;
        float posX;
        public delegate void OnDirChanged(Direction dir);
        public event OnDirChanged DirChanged;
        public DirectionOption(Direction dir, float posX)
        {
            this.dir = dir;
            this.posX = posX;
        }

        public void OnGUI()
        {
            if (GUI.Button(new Rect(posX, 100, 50, 50), Resources.Load<Texture>("Editor/Textures/" + dir.ToString())))
            {
                DirChanged(this.dir);
            }
        }




    }

    public interface IGUI
    {
        void OnGUI();
    }


    public class StringHelper
    {
        public static string getBetween(string strSource, string strStart, string strEnd)
        {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }
            else
            {
                return "";
            }

        }

    }
}