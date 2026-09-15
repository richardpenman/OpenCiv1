using Avalonia.Controls;
using Avalonia.Media;
using IRB.VirtualCPU;
using OpenCivOne.Graphics;
using OpenCivOne.Resources;
using SkiaSharp;
using System.Diagnostics;

namespace OpenCivOne
{
	public partial class OpenCivOneGame
	{
		public static readonly GPoint InvalidPosition = new GPoint(-1);

		private static bool enableLog = false;

		private Window mainWindow;
		private VCPU oCPU;

		#region Segment definitions
		private StartGame startGame;
		private Animations animations;
		private CommonTools commonTools;
		private GameEngine gameEngine;
		private MenuBoxDialog menuBoxDialog;
		private PlayerTurn playerTurn;
		private DrawTools drawTools;
		private ImageTools imageTools;
		private LanguageTools languageTools;
		private MapManagement mapManagement;
		private UnitManagement unitManagement;
		private UnitGoTo unitGoTo;
		private Actions oSegment_2459;
		private AIEngine aiEngine;
		private Segment_1ade oSegment_1ade;
		private CityWorker cityWorker;
		private AttackActions oSegment_29f3;
		private Diplomacy diplomacy;
		private Menus menus;
		private MainIntro mainIntro;
		private MeetWithKing meetWithKing;
		private MapInitAndIntro mapInitAndIntro;
		private Help help;
		private LoadAndSave loadAndSave;
		private HallOfFame hallOfFame;
		private StartGameMenu startGameMenu;
		private TextBoxDialogues textBoxDialogues;
		private Overlay_14 oOverlay_14;
		private Encyclopedia encyclopedia;
		private News news;
		private CityView cityView;
		private Overlay_18 oOverlay_18;
		private DiplomatActions diplomatActions;
		private Replay replay;
		private Reports reports;
		private Overlay_20 oOverlay_20;
		private Palace palace;
		private ShowDebugDetails showDebugDetails;
		private Secession secession;
		private CAPI CApi;
		private GDriver graphics;
		private NSound sound;
		#endregion

		private LogWrapper oLog;
		private LogWrapper oInterruptLog;
		private LogWrapper oGoToLog;

		private ushort usStartSegment = 0x1000;

		private GameData gameData;

		private string gameResourcePath = "";
		private string gameMainPath = "";
		private string gameSavePath = "";
		private GlobalPreferences globalPreferences;

		// Keyboard
		public static readonly object KeyboardAndMouseLock = new();
		private Queue<int> keys = new Queue<int>();

		// Mouse
		private List<MouseEvent> mouseEvents = new List<MouseEvent>();
		private MouseEvent lastMouseEvent = new MouseEvent(new GPoint(160, 100), MouseButtonsEnum.None);

		public OpenCivOneGame(Window mainWindow)
		{
			this.mainWindow = mainWindow;
			this.oLog = new LogWrapper($"{VCPU.AssemblyPath}Log.txt", enableLog);
			this.oInterruptLog = new LogWrapper($"{VCPU.AssemblyPath}InterruptLog.txt", enableLog);
			this.oGoToLog = new LogWrapper($"{VCPU.AssemblyPath}GoToLog.txt", enableLog);

			this.oCPU = new VCPU(this, this.oLog);

			this.oLog.CPU = this.oCPU;
			this.oInterruptLog.CPU = this.oCPU;
			this.oGoToLog.CPU = this.oCPU;

			this.gameData = new GameData();

			char separator = Path.DirectorySeparatorChar;
			string userProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

			try
			{
#if DEBUG
				if (Environment.OSVersion.Platform == PlatformID.Win32NT)
				{
					this.gameResourcePath = string.Format("C:{0}Dos{0}Civ1{0}", separator);
				}
				else
				{
					this.gameResourcePath = $"{userProfilePath}{separator}Dos{separator}Civ1{separator}";
				}
#else
				this.gameResourcePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{separator}";
#endif

				this.gameMainPath = $"{userProfilePath}{separator}Games{separator}OpenCivOne";

				if (!Path.Exists(gameMainPath))
				{
					Directory.CreateDirectory(gameMainPath);
				}

				this.gameSavePath = $"{gameMainPath}{separator}Saves";

				if (!Path.Exists(gameSavePath))
				{
					Directory.CreateDirectory(gameSavePath);
				}
			}
			catch (Exception e)
			{
				throw new Exception($"Could not create necessary game paths. The error is '{e.Message}'");
			}

			this.globalPreferences = GlobalPreferences.DeSerializeFromJSON(this, this.gameMainPath);

			#region Initialize Segments
			this.CApi = new CAPI(this);
			this.graphics = new GDriver(this);
			this.sound = new NSound(this);

			this.startGame = new StartGame(this);
			this.animations = new Animations(this);
			this.commonTools = new CommonTools(this);
			this.gameEngine = new GameEngine(this);
			this.menuBoxDialog = new MenuBoxDialog(this);
			this.playerTurn = new PlayerTurn(this);
			this.drawTools = new DrawTools(this);
			this.imageTools = new ImageTools(this);
			this.languageTools = new LanguageTools(this);
			this.mapManagement = new MapManagement(this);
			this.unitManagement = new UnitManagement(this);
			this.unitGoTo = new UnitGoTo(this);
			this.oSegment_2459 = new Actions(this);
			this.aiEngine = new AIEngine(this);
			this.oSegment_1ade = new Segment_1ade(this);
			this.cityWorker = new CityWorker(this);
			this.oSegment_29f3 = new AttackActions(this);
			this.diplomacy = new Diplomacy(this);
			this.menus = new Menus(this);
			this.mainIntro = new MainIntro(this);
			this.meetWithKing = new MeetWithKing(this);
			this.mapInitAndIntro = new MapInitAndIntro(this);
			this.help = new Help(this);
			this.loadAndSave = new LoadAndSave(this);
			this.hallOfFame = new HallOfFame(this);
			this.startGameMenu = new StartGameMenu(this);
			this.textBoxDialogues = new TextBoxDialogues(this);
			this.oOverlay_14 = new Overlay_14(this);
			this.encyclopedia = new Encyclopedia(this);
			this.news = new News(this);
			this.cityView = new CityView(this);
			this.oOverlay_18 = new Overlay_18(this);
			this.diplomatActions = new DiplomatActions(this);
			this.replay = new Replay(this);
			this.reports = new Reports(this);
			this.oOverlay_20 = new Overlay_20(this);
			this.palace = new Palace(this);
			this.showDebugDetails = new ShowDebugDetails(this);
			this.secession = new Secession(this);
			#endregion

			/*string[] aFiles = Directory.GetFiles(this.oParent.ResourcePath, "*.pic");

			for (int i = 0; i < aFiles.Length; i++)
			{
				WriteImage(aFiles[i], @"Z:\Civilization\Images\" + Path.GetFileNameWithoutExtension(aFiles[i]) + ".png");
			}*/

			/*byte[] palette;
			var colors = GBitmap.ReadPaletteFromPICFile(@"Z:\Games\Civilization\Civ I\Dos\Installed\CIV1.5\sp256.pal", out palette);

			StreamWriter writer = new StreamWriter("palette.txt");

			for (int i = 0; i < colors.Count; i++)
			{
				Color colour = colors[i].Value;

				writer.WriteLine($"new Color(0, {colour.R}, {colour.G}, {colour.B}),");
			}

			writer.Close();*/
		}

		private void WriteImage(string fileIn, string fileOut)
		{
			byte[] palette;
			GBitmap? originalBitmap = GBitmap.FromPICFile(fileIn, out palette, true);

			if (originalBitmap != null)
			{
				int width = originalBitmap.Width;
				int height = originalBitmap.Height;
				SKBitmap skBitmap = new SKBitmap(width, height, SKColorType.Rgba8888, SKAlphaType.Unpremul);

				for (int y = 0; y < height; y++)
				{
					for (int x = 0; x < width; x++)
					{
						Color color = originalBitmap.Palette[originalBitmap.GetPixel(x, y)];

						skBitmap.SetPixel(x, y, new SKColor(color.R, color.G, color.B));
					}
				}

				FileStream output = new FileStream(fileOut, FileMode.Create);
				skBitmap.Encode(output, SKEncodedImageFormat.Png, 80);
				output.Close();
			}
		}

		public void Start()
		{
			#region Check if all resources are present
			// Check for Default directory and individual Resource files
			if (!string.IsNullOrEmpty(this.gameResourcePath) && !Directory.Exists(this.gameResourcePath))
			{
				throw new ResourceMissingException($"Resource path not found at '{this.gameResourcePath}'.");
			}

			string[] aResourceFiles = new string[] {
				"adscreen.pic", "arch.pic", "back0a.pic", "back0m.pic", "back1a.pic", "back1m.pic", "back2a.pic", "back2m.pic",
				"back3a.pic", "birth0.pic", "birth1.pic", "birth2.pic", "birth3.pic", "birth4.pic", "birth5.pic", "birth6.pic",
				"birth7.pic", "birth8.pic", "castle0.pic", "castle1.pic", "castle2.pic", "castle3.pic", "castle4.pic", "cback.pic",
				"cbacks1.pic", "cbacks2.pic", "cbacks3.pic", "cbrush0.pic", "cbrush1.pic", "cbrush2.pic", "cbrush3.pic",
				"cbrush4.pic", "cbrush5.pic", "civ.exe", "citypix1.pic", "citypix2.pic", "citypix3.pic", "custom.pic", "diffs.pic",
				"discovr1.pic", "discovr2.pic", "docker.pic", "govt0a.pic", "govt0m.pic", "govt1a.pic", "govt1m.pic", "govt2a.pic",
				"govt2m.pic", "govt3a.pic", "hill.pic", "iconpg1.pic", "iconpg2.pic", "iconpg3.pic", "iconpg4.pic", "iconpg5.pic",
				"iconpg6.pic", "iconpg7.pic", "iconpg8.pic", "iconpga.pic", "iconpgb.pic", "iconpgc.pic", "iconpgd.pic",
				"iconpge.pic", "iconpgt1.pic", "iconpgt2.pic", "invader2.pic", "invader3.pic", "invaders.pic", "king00.pic",
				"king01.pic", "king02.pic", "king03.pic", "king04.pic", "king05.pic", "king06.pic", "king07.pic", "king08.pic",
				"king09.pic", "king10.pic", "king11.pic", "king12.pic", "king13.pic", "kink00.pic", "kink03.pic", "logo.pic",
				"love1.pic", "love2.pic", "map.pic", "nuke1.pic", "planet1.pic", "planet2.pic", "pop.pic", "riot.pic", "riot2.pic",
				"sad.pic", "settlers.pic", "slag2.pic", "slam1.pic", "slam2.pic", "sp257.pic", "sp299.pic", "spacest.pic",
				"sprites.pic", "ter257.pic", "torch.pic", "wonders.pic", "wonders2.pic", "back0a.pal", "back0m.pal", "back1a.pal",
				"back1m.pal", "back2a.pal", "back2m.pal", "back3a.pal", "birth0.pal", "birth1.pal", "birth2.pal", "birth3.pal",
				"birth4.pal", "birth5.pal", "birth6.pal", "birth7.pal", "birth8.pal", "discovr1.pal", "discovr2.pal", "hill.pal",
				"iconpg1.pal", "iconpga.pal", "king00.pal", "king01.pal", "king02.pal", "king03.pal", "king04.pal", "king05.pal",
				"king06.pal", "king07.pal", "king08.pal", "king09.pal", "king10.pal", "king11.pal", "king12.pal", "king13.pal",
				"slam1.pal", "sp256.pal", "sp257.pal", "blurb0.txt", "blurb1.txt", "blurb2.txt", "blurb3.txt", "blurb4.txt",
				"credits.txt", "error.txt", "help.txt", "intro.txt", "intro3.txt", "king.txt", "produce.txt", "story.txt"
			};

			for (int i = 0; i < aResourceFiles.Length; i++)
			{
				string sFilePath = $"{this.gameResourcePath}{aResourceFiles[i].ToUpper()}";

				if (!File.Exists(sFilePath))
				{
					throw new ResourceMissingException($"Missing resource file {sFilePath}. Please ensure that the file exists at specified path.");
				}
			}
			#endregion

			//ushort usInitialCS = 0x2045; // oEXE.InitialCS;
			ushort usInitialSS = 0x398d; // oEXE.InitialSS;
			ushort usInitialSP = 0x0800; // oEXE.InitialSP;

			this.oCPU.Memory.AllocateMemoryBlock(0xff00, 0x100, VCPUMemoryFlagsEnum.ReadWrite);
			this.oCPU.Memory.WriteUInt16(0xff00, 0x20cd);
			this.oCPU.Memory.WriteUInt16(0xff02, (ushort)(this.oCPU.Memory.FreeMemory.End >> 4));
			this.oCPU.Memory.WriteUInt8(0xff81, (byte)'\r');
			this.oCPU.Memory.MemoryRegions[1].AccessFlags = VCPUMemoryFlagsEnum.ReadWrite | VCPUMemoryFlagsEnum.WriteWarning | VCPUMemoryFlagsEnum.ReadWarning;

			uint uiEXEStart = VCPU.ToLinearAddress(usStartSegment, 0);
			uint uiEXELength = 0x3a0e0;

			byte[] resources = CommonResources.BinaryResources;

			this.oCPU.Memory.AllocateMemoryBlock(uiEXEStart, uiEXELength, VCPUMemoryFlagsEnum.ReadWrite);
			this.oCPU.Memory.WriteBlock(VCPU.ToLinearAddress(0x35cf, 0), resources, 0, resources.Length);
			this.oCPU.Memory.MemoryRegions[2].AccessFlags |= VCPUMemoryFlagsEnum.WriteWarning;

			// Define data and stack region
			uint uiDataStart = uiEXEStart + VCPU.ToLinearAddress(0x25cf, 0);
			uint uiDataEnd = uiEXEStart + VCPU.ToLinearAddress(0x2b01, 0xf0c0);

			this.oCPU.Memory.MemoryRegions[2].End = uiDataStart - 1;
			this.oCPU.Memory.MemoryRegions.Add(
				new VCPUMemoryRegion(uiDataStart, (uiDataEnd - uiDataStart) + 1, VCPUMemoryFlagsEnum.ReadWrite));

			// Initialize CPU
			//this.oCPU.CS.Word = (ushort)(usInitialCS + usStartSegment);
			this.oCPU.SS.UInt16 = (ushort)(usInitialSS + usStartSegment);
			this.oCPU.DS.UInt16 = (ushort)(usStartSegment - 0x10);
			this.oCPU.ES.UInt16 = (ushort)(usStartSegment - 0x10);
			this.oCPU.SP.UInt16 = usInitialSP;

			ushort usDataSegment = 0x3b01;

			// function body
			this.oCPU.PUSH_UInt16(this.oCPU.DS.UInt16);

			this.oCPU.DS.UInt16 = 0x3b01;

			// Not important, but just for case it's still needed, to be removed later
			string sPath = $"{this.gameResourcePath}CIV.EXE";

			this.oCPU.WriteUInt8(this.oCPU.DS.UInt16, 0x61ee, (byte)'C');
			this.oCPU.WriteString(VCPU.ToLinearAddress(this.oCPU.DS.UInt16, 0x6156), sPath, sPath.Length);

			this.oCPU.DS.UInt16 = this.oCPU.POP_UInt16();
			this.oCPU.ES.UInt16 = this.oCPU.DS.UInt16;

			this.oCPU.SI.UInt16 = (ushort)(this.oCPU.Memory.FreeMemory.End >> 4); // top of memory
			this.oCPU.SI.UInt16 = this.oCPU.SUB_UInt16(this.oCPU.SI.UInt16, usDataSegment);

			// Init SS:SP
			this.oCPU.CLI();
			this.oCPU.SS.UInt16 = usDataSegment;
			this.oCPU.SP.UInt16 = this.oCPU.ADD_UInt16(this.oCPU.SP.UInt16, 0xe8c0);
			this.oCPU.STI();

			// Align SP
			this.oCPU.SP.UInt16 = this.oCPU.AND_UInt16(this.oCPU.SP.UInt16, 0xfffe);

			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, 0x5890, this.oCPU.SP.UInt16);
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, 0x588c, this.oCPU.SP.UInt16);

			this.oCPU.AX.UInt16 = this.oCPU.SI.UInt16;
			this.oCPU.AX.UInt16 = this.oCPU.SHL_UInt16(this.oCPU.AX.UInt16, 4);
			this.oCPU.AX.UInt16 = this.oCPU.DEC_UInt16(this.oCPU.AX.UInt16);
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, 0x588a, this.oCPU.AX.UInt16);

			this.oCPU.SI.UInt16 = this.oCPU.ADD_UInt16(this.oCPU.SI.UInt16, usDataSegment);
			this.oCPU.BX.UInt16 = this.oCPU.ES.UInt16;
			this.oCPU.BX.UInt16 = this.oCPU.SUB_UInt16(this.oCPU.BX.UInt16, this.oCPU.SI.UInt16);
			this.oCPU.BX.UInt16 = this.oCPU.NEG_UInt16(this.oCPU.BX.UInt16);
			if (this.oCPU.Memory.ResizeMemoryBlock(this.oCPU.ES.UInt16, this.oCPU.BX.UInt16))
			{
				this.oCPU.Flags.C = false;
				this.oCPU.AX.UInt16 = 0;
			}
			else
			{
				this.oCPU.Flags.C = true;
				this.oCPU.AX.UInt16 = 8;
				this.oCPU.BX.UInt16 = 0;
			}

			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, 0x5901, this.oCPU.DS.UInt16);

			this.oCPU.ES.UInt16 = this.oCPU.SS.UInt16;
			this.oCPU.DS.UInt16 = this.oCPU.SS.UInt16;

			// Clear the rest of data and stack segment 0x652e - 0xe8c0
			for (int i = 0x652e; i < this.oCPU.SP.UInt16; i++)
			{
				if (i < 0x70ec && i > 0x70ec + 0xdff)
					this.oCPU.WriteUInt8(usDataSegment, (ushort)i, 0);
			}

			// DOS version
			this.oCPU.WriteUInt16(this.oCPU.DS.UInt16, 0x5903, 0x616);

			// Environment block is not used
			// Argument block is not used
			this.oCPU.WriteUInt16(this.oCPU.DS.UInt16, 0x5922, 0);
			this.oCPU.WriteUInt16(this.oCPU.DS.UInt16, 0x5920, 0);
			this.oCPU.WriteUInt16(this.oCPU.DS.UInt16, 0x591e, 0);

			this.oCPU.BP.UInt16 = 0x0;

			// Call our StartGame function
			this.StartGame.StartMainGame();

			this.CAPI.exit(0);
		}

		public MouseEvent PeekMouseEvent()
		{
			this.oCPU.DoEvents();

			lock (OpenCivOneGame.KeyboardAndMouseLock)
			{
				if (this.mouseEvents.Count > 0)
				{
					MouseEvent mouseEvent = this.mouseEvents[0];

					// We can safely remove mouse movement event
					if (mouseEvent.Buttons == MouseButtonsEnum.None)
					{
						this.mouseEvents.RemoveAt(0);
						this.lastMouseEvent = mouseEvent;
					}
					
					return mouseEvent;
				}

				return this.lastMouseEvent;
			}
		}

		public bool HasNewMouseEvent()
		{
			this.oCPU.DoEvents();

			lock (OpenCivOneGame.KeyboardAndMouseLock)
			{
				if (this.mouseEvents.Count > 0)
				{
					return true;
				}

				return false;
			}
		}

		public MouseEvent GetMouseEvent()
		{
			this.oCPU.DoEvents();

			lock (OpenCivOneGame.KeyboardAndMouseLock)
			{
				if (this.mouseEvents.Count > 0)
				{
					MouseEvent mouseEvent = this.mouseEvents[0];
					this.mouseEvents.RemoveAt(0);
					this.lastMouseEvent = mouseEvent;

					return mouseEvent;
				}

				return this.lastMouseEvent;
			}
		}

		/// <summary>
		/// Clears the mouse buffer and ensures no mouse button is pressed
		/// </summary>
		public void ClearMouseEventsAndEnsureNoMouseButtonIsPressed()
		{
			lock (OpenCivOneGame.KeyboardAndMouseLock)
			{
				if (this.mouseEvents.Count > 0)
				{
					this.lastMouseEvent = this.mouseEvents[this.mouseEvents.Count - 1];
				}

				this.mouseEvents.Clear();
			}

			while (this.GetMouseEvent().Buttons != MouseButtonsEnum.None) { Thread.Sleep(1); }
		}

		public Window MainWindow { get => this.mainWindow; }

		public VCPU CPU { get => this.oCPU; }

		public GameData GameData { get => this.gameData; }

		public string ResourcePath { get => this.gameResourcePath; }

		public string MainPath { get => this.gameMainPath; }

		public string SavePath { get => this.gameSavePath; }

		public GlobalPreferences GlobalPreferences { get => this.globalPreferences; }

		public Queue<int> Keys
		{
			get { return this.keys; }
		}

		public List<MouseEvent> MouseEvents
		{
			get { return this.mouseEvents; }
		}

		#region Logs
		public LogWrapper Log
		{
			get { return this.oLog; }
		}

		public LogWrapper InterruptLog
		{
			get { return this.oInterruptLog; }
		}

		public LogWrapper GoToLog
		{
			get { return this.oGoToLog; }
		}

		public static void LogUnit(OpenCivOneGame game, LogWrapper log, int playerID, int unitID, int humanPlayerID)
		{
			if (playerID >= game.GameData.Players.Length || unitID >= game.GameData.Players[playerID].Units.Length)
			{
				log.WriteLine($"// Illegal indexes, PlayerID: {playerID}{((playerID == humanPlayerID) ? " (Human player)" : "")}, UnitID: {unitID}");
			}
			else
			{
				Unit unit = game.GameData.Players[playerID].Units[unitID];

				log.WriteLine($"// Player[{playerID}{((playerID == humanPlayerID) ? " (Human player)" : "")}].Unit[{unitID}] = {{TypeID: {unit.UnitType}, Status: {unit.Status}, Position: {unit.Position}, GoTo: {unit.GoToDestination}}}");
			}
		}
		#endregion

		#region Public Segment objects
		public StartGame StartGame { get => this.startGame; }

		public CommonTools CommonTools { get => this.commonTools; }

		public Animations Animations { get => this.animations; }

		public GameEngine GameEngine { get => this.gameEngine; }

		public MenuBoxDialog MenuBoxDialog
		{
			get { return this.menuBoxDialog; }
		}

		public PlayerTurn PlayerTurn
		{
			get { return this.playerTurn; }
		}

		public DrawTools DrawTools
		{
			get { return this.drawTools; }
		}

		public ImageTools ImageTools
		{
			get { return this.imageTools; }
		}

		public LanguageTools LanguageTools
		{
			get { return this.languageTools; }
		}

		public MapManagement MapManagement
		{
			get { return this.mapManagement; }
		}

		public UnitManagement UnitManagement
		{
			get { return this.unitManagement; }
		}

		public UnitGoTo UnitGoTo
		{
			get { return this.unitGoTo; }
		}

		public Actions Segment_2459
		{
			get { return this.oSegment_2459; }
		}

		public AIEngine AIEngine
		{
			get { return this.aiEngine; }
		}

		public Segment_1ade Segment_1ade
		{
			get { return this.oSegment_1ade; }
		}

		public CityWorker CityWorker
		{
			get { return this.cityWorker; }
		}

		public AttackActions Segment_29f3
		{
			get { return this.oSegment_29f3; }
		}

		public Diplomacy Diplomacy
		{
			get { return this.diplomacy; }
		}

		public Menus Menus
		{
			get { return this.menus; }
		}

		public MainIntro MainIntro
		{
			get { return this.mainIntro; }
		}

		public MeetWithKing MeetWithKing
		{
			get { return this.meetWithKing; }
		}

		public MapInitAndIntro MapInitAndIntro
		{
			get { return this.mapInitAndIntro; }
		}

		public Help Help
		{
			get { return this.help; }
		}

		public LoadAndSave LoadAndSave
		{
			get { return this.loadAndSave; }
		}

		public HallOfFame HallOfFame
		{
			get { return this.hallOfFame; }
		}

		public StartGameMenu StartGameMenu
		{
			get { return this.startGameMenu; }
		}

		public TextBoxDialogues TextBoxDialogues
		{
			get { return this.textBoxDialogues; }
		}

		public Overlay_14 Overlay_14
		{
			get { return this.oOverlay_14; }
		}

		public Encyclopedia Encyclopedia
		{
			get { return this.encyclopedia; }
		}

		public News News
		{
			get { return this.news; }
		}

		public CityView CityView
		{
			get { return this.cityView; }
		}

		public Overlay_18 Overlay_18
		{
			get { return this.oOverlay_18; }
		}

		public DiplomatActions DiplomatActions
		{
			get { return this.diplomatActions; }
		}

		public Replay Replay
		{
			get { return this.replay; }
		}

		public Reports Reports
		{
			get { return this.reports; }
		}

		public Overlay_20 Overlay_20
		{
			get { return this.oOverlay_20; }
		}

		public Palace Palace
		{
			get { return this.palace; }
		}

		public ShowDebugDetails ShowDebugDetails
		{
			get { return this.showDebugDetails; }
		}

		public Secession Secession
		{
			get { return this.secession; }
		}

		public CAPI CAPI
		{
			get { return this.CApi; }
		}

		public GDriver Graphics
		{
			get { return this.graphics; }
		}

		public NSound Sound
		{
			get { return this.sound; }
		}
		#endregion
	}
}
