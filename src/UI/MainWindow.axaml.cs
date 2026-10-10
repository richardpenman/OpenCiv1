using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using IRB.Collections.Generic;
using IRB.VirtualCPU;
using OpenCivOne.Graphics;
using System.Runtime.InteropServices;
using ThreadState = System.Threading.ThreadState;

namespace OpenCivOne.UI
{
	public partial class MainWindow : Window
	{
		private bool closing = false;

		private OpenCivOneGame game;
		private DispatcherTimer timer;
		private Thread gameThread;
		private Exception? gameException = null;

		// track how many screens, columns and rows we have to show
		private int screenCount = 0;
		private int screenColumns = 1;
		private int screenRows = 1;

		// Main screen dimensions
		private GSize screenSize = new GSize(320, 200);
		private GSize imageSize = new GSize(320, 200);

		// Mouse region
		private GRectangle mouseRect = new GRectangle();
		private MouseEvent lastMouseEvent = new MouseEvent(new GPoint(160, 100), MouseButtonsEnum.None);

		// We have two interchangeable buffer bitmaps
		private int bitmapIndex = 0;
		private WriteableBitmap[] bitmaps = new WriteableBitmap[2];

		public MainWindow()
		{
			InitializeComponent();

#if !DEBUG
			MessageBox.Show("This Alpha Release of OpenCivOne project " +
				"most certainly has bugs, but most functions should work normally, and has no sound at this point. " +
				"It is compatible with old civ.exe and can save/load original game files.\n\n\n" +
				"Technicalities:\n\nTHE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR " +
				"IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, " +
				"FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE " +
				"AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER " +
				"LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, " +
				"OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.",
				"Warning", MessageBoxIcon.Warning, MessageBoxButtons.OK);
#endif

			Thread.CurrentThread.Name = "OpenCivOne main thread";

			// Initialize game state
			this.game = new OpenCivOneGame(this);

			// Main Windows events
			this.Closing += this.MainWindow_Closing;
			this.KeyDown += this.MainWindow_KeyDown;

			this.PointerMoved += this.MainWindow_PointerMoved;
			this.PointerPressed += this.MainWindow_PointerPressed;
			this.PointerReleased += this.MainWindow_PointerReleased;

			//this.mainImageBorder.PointerMoved += this.MainWindow_PointerMoved;
			//this.mainImageBorder.PointerPressed += this.MainWindow_PointerPressed;
			//this.mainImageBorder.PointerReleased += this.MainWindow_PointerReleased;

			// Initialize Window refresh timer
			this.timer = new DispatcherTimer(DispatcherPriority.Normal);
			this.timer.Interval = TimeSpan.FromMilliseconds(20);
			this.timer.Tick += this.Timer_Tick;
			this.timer.Start();

			// Initialize game thread where we have all the fun ;)
			this.gameThread = new Thread(new ThreadStart(GameThread));
			this.gameThread.Name = "OpenCivOne game thread";
			this.gameThread.Start();
		}

		private void Timer_Tick(object? sender, EventArgs e)
		{
			if (!this.closing && this.gameThread.ThreadState == ThreadState.Stopped)
			{
				if (this.gameException != null)
				{
					Exception ex = this.gameException;
					if (ex is ResourceMissingException)
					{
						MessageBox.Show(this, ex.Message, "Game resource error", MessageBoxIcon.Error, MessageBoxButtons.OK);
					}
					else
					{
						MessageBox.Show(this, "There was an error in the OpenCivOne game engine, " +
							"the details about the error will be in the Exception.log file.", "Game engine error", MessageBoxIcon.Error, MessageBoxButtons.OK);

						try
						{
							StreamWriter writer = new StreamWriter($"{VCPU.AssemblyPath}Exception.log", true);

							writer.WriteLine("---------------------------");
							writer.WriteLine($"Message: {ex.Message}");
							writer.WriteLine($"Source: {ex.Source}");
							writer.WriteLine($"Stack trace: {ex.StackTrace}");

							writer.Close();
						}
						catch (Exception ex1)
						{
							MessageBox.Show(this, $"Could not write the Exception.log file. The exception (when trying to open a file) was: {ex1.Message}",
								"Game engine error", MessageBoxIcon.Error, MessageBoxButtons.OK);
						}
					}
				}

				this.closing = true;
				this.Close();
			}

			if (!this.closing)
			{
				if (this.game.CPU.Pause && !this.gamePaused.IsVisible)
				{
					this.gamePaused.IsVisible = true;
				}
				else if (!this.game.CPU.Pause && this.gamePaused.IsVisible)
				{
					this.gamePaused.IsVisible = false;
				}

				RedrawScreens(false);
			}
		}

		#region Keyboard and Mouse events
		private void MainWindow_KeyDown(object? sender, Avalonia.Input.KeyEventArgs e)
		{
			lock (OpenCivOneGame.KeyboardAndMouseLock)
			{
				e.Handled = true;

				if (e.KeyModifiers == KeyModifiers.None)
				{
					switch (e.Key)
					{
						/*case Key.NumPad0:
							// for testing
							this.game.Keys.Enqueue(0x475c);
							break;*/
						/*case Key.Tab:
							this.game.Keys.Enqueue(0x9);
							break;*/

						case Key.Escape:
							this.game.Keys.Enqueue(0x1b);
							break;

						case Key.F1:
							this.game.Keys.Enqueue(0x3b00);
							break;

						case Key.F2:
							this.game.Keys.Enqueue(0x3c00);
							break;

						case Key.F3:
							this.game.Keys.Enqueue(0x3d00);
							break;

						case Key.F4:
							this.game.Keys.Enqueue(0x3e00);
							break;

						case Key.F5:
							this.game.Keys.Enqueue(0x3f00);
							break;

						case Key.F6:
							this.game.Keys.Enqueue(0x4000);
							break;

						case Key.F7:
							this.game.Keys.Enqueue(0x4100);
							break;

						case Key.F8:
							this.game.Keys.Enqueue(0x4200);
							break;

						case Key.F9:
							this.game.Keys.Enqueue(0x4300);
							break;

						case Key.F10:
							this.game.Keys.Enqueue(0x4400);
							break;

						case Key.Down:
						case Key.NumPad2:
							this.game.Keys.Enqueue(0x5000);
							break;

						case Key.Left:
						case Key.NumPad4:
							this.game.Keys.Enqueue(0x4b00);
							break;

						case Key.Right:
						case Key.NumPad6:
							this.game.Keys.Enqueue(0x4d00);
							break;

						case Key.Up:
						case Key.NumPad8:
							this.game.Keys.Enqueue(0x4800);
							break;

						case Key.Home:
						case Key.NumPad7:
							this.game.Keys.Enqueue(0x4700);
							break;

						case Key.End:
						case Key.NumPad1:
							this.game.Keys.Enqueue(0x4f00);
							break;

						case Key.PageUp:
						case Key.NumPad9:
							this.game.Keys.Enqueue(0x4900);
							break;

						case Key.PageDown:
						case Key.NumPad3:
							this.game.Keys.Enqueue(0x5100);
							break;

						default:
							if (e.KeySymbol != null && e.KeySymbol.Length > 0)
							{
								this.game.Keys.Enqueue((int)e.KeySymbol[0]);
							}
							else
							{
								e.Handled = false;
							}
							break;
					}
				}
				else if ((e.KeyModifiers & KeyModifiers.Shift) == KeyModifiers.Shift)
				{
					switch (e.Key)
					{
						case Key.Down:
							this.game.Keys.Enqueue(0x5032);
							break;

						case Key.Left:
							this.game.Keys.Enqueue(0x4b34);
							break;

						case Key.Right:
							this.game.Keys.Enqueue(0x4d36);
							break;

						case Key.Up:
							this.game.Keys.Enqueue(0x4838);
							break;

						case Key.Home:
							this.game.Keys.Enqueue(0x4737);
							break;

						case Key.End:
							this.game.Keys.Enqueue(0x4f31);
							break;

						case Key.PageUp:
							this.game.Keys.Enqueue(0x4939);
							break;

						case Key.PageDown:
							this.game.Keys.Enqueue(0x5133);
							break;

						default:
							if (e.KeySymbol != null && e.KeySymbol.Length > 0)
							{
								this.game.Keys.Enqueue((int)e.KeySymbol[0]);
							}
							else
							{
								e.Handled = false;
							}
							break;
					}
				}
				else if ((e.KeyModifiers & KeyModifiers.Alt) == KeyModifiers.Alt)
				{
					switch (e.Key)
					{
						case Key.D1:
							ToggleScreen(0);
							break;

						case Key.D2:
							ToggleScreen(1);
							break;

						case Key.D3:
							ToggleScreen(2);
							break;

						case Key.D4:
							ToggleScreen(3);
							break;

						case Key.A:
							this.game.Keys.Enqueue(0x1e00);
							break;

						case Key.D:
							this.game.Keys.Enqueue(0x2000);
							break;

						case Key.E:
							this.game.Keys.Enqueue(0x2e00);
							break;

						case Key.F:
							this.game.Keys.Enqueue(0x2001);
							break;

						case Key.G:
							this.game.Keys.Enqueue(0x2200);
							break;

						case Key.H:
							this.game.Keys.Enqueue(0x2300);
							break;

						case Key.M:
							this.game.Keys.Enqueue(0x3200);
							break;

						case Key.O:
							this.game.Keys.Enqueue(0x1800);
							break;

						case Key.P:
							this.game.CPU.Pause = !this.game.CPU.Pause;
							break;

						case Key.Q:
							this.game.Keys.Enqueue(0x1000);
							break;

						case Key.R:
							this.game.Keys.Enqueue(0x1300);
							break;

						case Key.V:
							this.game.Keys.Enqueue(0x2f00);
							break;

						case Key.W:
							this.game.Keys.Enqueue(0x1100);
							break;

						default:
							e.Handled = false;
							break;
					}
				}
			}
		}

		private void MainWindow_PointerMoved(object? sender, PointerEventArgs e)
		{
			PointerPoint pointer = e.GetCurrentPoint(this.mainImage);
			Point point = pointer.Position;
			GPoint location = new GPoint((int)point.X, (int)point.Y);

			if (this.mouseRect.Contains(location))
			{
				GPoint currentPosition = new GPoint(location.X / (this.screenSize.Width / 320), location.Y / (this.screenSize.Height / 200));
				MouseButtonsEnum currentButtons = (pointer.Properties.IsLeftButtonPressed ? MouseButtonsEnum.Left : MouseButtonsEnum.None) |
					(pointer.Properties.IsRightButtonPressed ? MouseButtonsEnum.Right : MouseButtonsEnum.None);

				// prevent repetitive events
				if (this.lastMouseEvent.Position != currentPosition || this.lastMouseEvent.Buttons != currentButtons)
				{
					this.lastMouseEvent = new MouseEvent(currentPosition, currentButtons);

					lock (OpenCivOneGame.KeyboardAndMouseLock)
					{
						if (currentButtons == MouseButtonsEnum.None)
						{
							if (this.game.MouseEvents.Count > 0)
							{
								MouseEvent mouseEvent = this.game.MouseEvents[this.game.MouseEvents.Count - 1];

								if (mouseEvent.Buttons == MouseButtonsEnum.None)
								{
									//Debug.WriteLine($"Removed mouse event [{mouseEvent.Position}, {mouseEvent.Buttons}] ({this.game.MouseEvents.Count})");
									this.game.MouseEvents.RemoveAt(this.game.MouseEvents.Count - 1);
								}
							}

							//Debug.WriteLine($"Move [{currentPosition}, {currentButtons}] ({this.game.MouseEvents.Count})");
							this.game.MouseEvents.Add(new MouseEvent(currentPosition, currentButtons));
						}
						else
						{
							if (this.game.MouseEvents.Count > 0)
							{
								MouseEvent mouseEvent = this.game.MouseEvents[this.game.MouseEvents.Count - 1];

								if (mouseEvent.Buttons != currentButtons)
								{
									//Debug.WriteLine($"Move [{currentPosition}, {currentButtons}] ({this.game.MouseEvents.Count})");
									this.game.MouseEvents.Add(new MouseEvent(currentPosition, currentButtons));
								}
							}
							else
							{
								//Debug.WriteLine($"Move [{currentPosition}, {currentButtons}] ({this.game.MouseEvents.Count})");
								this.game.MouseEvents.Add(new MouseEvent(currentPosition, currentButtons));
							}
						}
					}
				}
			}

			e.Handled = true;
		}

		private void MainWindow_PointerPressed(object? sender, PointerPressedEventArgs e)
		{
			PointerPoint pointer = e.GetCurrentPoint(this.mainImage);
			Point point = pointer.Position;
			GPoint location = new GPoint((int)point.X, (int)point.Y);

			if (this.mouseRect.Contains(location))
			{
				GPoint currentPosition = new GPoint(location.X / (this.screenSize.Width / 320), location.Y / (this.screenSize.Height / 200));
				MouseButtonsEnum currentButtons = (pointer.Properties.IsLeftButtonPressed ? MouseButtonsEnum.Left : MouseButtonsEnum.None) |
					(pointer.Properties.IsRightButtonPressed ? MouseButtonsEnum.Right : MouseButtonsEnum.None);

				// prevent repetitive events
				if (this.lastMouseEvent.Position != currentPosition || this.lastMouseEvent.Buttons != currentButtons)
				{
					this.lastMouseEvent = new MouseEvent(currentPosition, currentButtons);

					lock (OpenCivOneGame.KeyboardAndMouseLock)
					{

						if (currentButtons == MouseButtonsEnum.None)
						{
							if (this.game.MouseEvents.Count > 0)
							{
								MouseEvent mouseEvent = this.game.MouseEvents[this.game.MouseEvents.Count - 1];

								if (mouseEvent.Buttons == MouseButtonsEnum.None)
								{
									//Debug.WriteLine($"Removed mouse event [{mouseEvent.Position}, {mouseEvent.Buttons}] ({this.game.MouseEvents.Count})");
									this.game.MouseEvents.RemoveAt(this.game.MouseEvents.Count - 1);
								}
							}

							//Debug.WriteLine($"Press [{currentPosition}, {currentButtons}] ({this.game.MouseEvents.Count})");
							this.game.MouseEvents.Add(new MouseEvent(currentPosition, currentButtons));
						}
						else
						{
							if (this.game.MouseEvents.Count > 0)
							{
								MouseEvent mouseEvent = this.game.MouseEvents[this.game.MouseEvents.Count - 1];

								if (mouseEvent.Buttons != currentButtons)
								{
									//Debug.WriteLine($"Press [{currentPosition}, {currentButtons}] ({this.game.MouseEvents.Count})");
									this.game.MouseEvents.Add(new MouseEvent(currentPosition, currentButtons));
								}
							}
							else
							{
								//Debug.WriteLine($"Press [{currentPosition}, {currentButtons}] ({this.game.MouseEvents.Count})");
								this.game.MouseEvents.Add(new MouseEvent(currentPosition, currentButtons));
							}
						}
					}
				}
			}

			e.Handled = true;
		}

		private void MainWindow_PointerReleased(object? sender, PointerReleasedEventArgs e)
		{
			PointerPoint pointer = e.GetCurrentPoint(this.mainImage);
			Point point = pointer.Position;
			GPoint location = new GPoint((int)point.X, (int)point.Y);

			if (this.mouseRect.Contains(location))
			{
				GPoint currentPosition = new GPoint(location.X / (this.screenSize.Width / 320), location.Y / (this.screenSize.Height / 200));
				MouseButtonsEnum currentButtons = (pointer.Properties.IsLeftButtonPressed ? MouseButtonsEnum.Left : MouseButtonsEnum.None) |
					(pointer.Properties.IsRightButtonPressed ? MouseButtonsEnum.Right : MouseButtonsEnum.None);

				// prevent repetitive events
				if (this.lastMouseEvent.Position != currentPosition || this.lastMouseEvent.Buttons != currentButtons)
				{
					this.lastMouseEvent = new MouseEvent(currentPosition, currentButtons);

					lock (OpenCivOneGame.KeyboardAndMouseLock)
					{
						if (currentButtons == MouseButtonsEnum.None)
						{
							if (this.game.MouseEvents.Count > 0)
							{
								MouseEvent mouseEvent = this.game.MouseEvents[this.game.MouseEvents.Count - 1];

								if (mouseEvent.Buttons == MouseButtonsEnum.None)
								{
									//Debug.WriteLine($"Removed mouse event [{mouseEvent.Position}, {mouseEvent.Buttons}] ({this.game.MouseEvents.Count})");
									this.game.MouseEvents.RemoveAt(this.game.MouseEvents.Count - 1);
								}
							}

							//Debug.WriteLine($"Release [{currentPosition}, {currentButtons}] ({this.game.MouseEvents.Count})");
							this.game.MouseEvents.Add(new MouseEvent(currentPosition, currentButtons));
						}
						else
						{
							if (this.game.MouseEvents.Count > 0)
							{
								MouseEvent mouseEvent = this.game.MouseEvents[this.game.MouseEvents.Count - 1];

								if (mouseEvent.Buttons != currentButtons)
								{
									//Debug.WriteLine($"Release [{currentPosition}, {currentButtons}] ({this.game.MouseEvents.Count})");
									this.game.MouseEvents.Add(new MouseEvent(currentPosition, currentButtons));
								}
							}
							else
							{
								//Debug.WriteLine($"Release [{currentPosition}, {currentButtons}] ({this.game.MouseEvents.Count})");
								this.game.MouseEvents.Add(new MouseEvent(currentPosition, currentButtons));
							}
						}
					}
				}
			}

			e.Handled = true;
		}
		#endregion

		private void MainWindow_Closing(object? sender, WindowClosingEventArgs e)
		{
			if (!this.closing)
			{
				if (MessageBox.Show(this, "Are you sure you want to exit OpenCivOne?\nCurrent game, if any, will not be automatically saved!", "Exit application",
					MessageBoxIcon.Question, MessageBoxButtons.OKCancel, MessageBoxDefaultButton.Button2) == MessageBoxResult.OK)
				{
					this.closing = true;
					this.game.CPU.OnApplicationExit();
				}
				else
				{
					e.Cancel = true;
				}
			}
		}

		private void ResizeWindowAndImage()
		{
			switch (this.screenCount)
			{
				case 0:
				case 1:
					this.screenColumns = 1;
					this.screenRows = 1;
					break;

				case 2:
					this.screenColumns = 2;
					this.screenRows = 1;
					break;

				case 3:
					this.screenColumns = 2;
					this.screenRows = 2;
					break;
			}
			this.imageSize = new GSize(this.screenSize.Width * this.screenColumns, this.screenSize.Height * this.screenRows);

			this.mainImage.Width = this.imageSize.Width;
			this.mainImage.Height = this.imageSize.Height;

			this.mouseRect = new GRectangle(0, 0, this.screenSize);

			this.bitmaps[0] = new WriteableBitmap(new PixelSize(this.imageSize.Width, this.imageSize.Height), new Vector(96, 96), PixelFormat.Bgra8888);
			this.bitmaps[1] = new WriteableBitmap(new PixelSize(this.imageSize.Width, this.imageSize.Height), new Vector(96, 96), PixelFormat.Bgra8888);
		}

		private void ToggleScreen(int screen)
		{
			if (this.game != null)
			{
				if (this.game.Graphics.Screens.ContainsKey(screen))
				{
					GBitmap oScreen = this.game.Graphics.Screens.GetValueByKey(screen);

					oScreen.Visible = !oScreen.Visible;
				}
			}
		}

		private void RedrawScreens(bool forceRedraw)
		{
			if (this.game != null)
			{
				lock (VCPU.GraphicsLock)
				{
					int iColumn = 0;
					int iRow = 0;

					int iNewScreenCount = 0;
					bool bNeedsRedraw = false;

					for (int i = 0; i < this.game.Graphics.Screens.Count; i++)
					{
						if (this.game.Graphics.Screens[i].Value.Visible)
						{
							iNewScreenCount++;

							if (this.game.Graphics.Screens[i].Value.Modified)
								bNeedsRedraw = true;
						}
					}

					if (this.screenCount != iNewScreenCount)
					{
						this.screenCount = iNewScreenCount;
						ResizeWindowAndImage();
						forceRedraw = true;
					}

					if (bNeedsRedraw || forceRedraw)
					{
						WriteableBitmap currentBitmap = this.bitmaps[this.bitmapIndex];
						int iPixelByteSize = 4;
						int iStride = this.imageSize.Width * iPixelByteSize;

						using (ILockedFramebuffer l = currentBitmap.Lock())
						{
							for (int i = 0; i < this.game.Graphics.Screens.Count; i++)
							{
								BKeyValuePair<int, GBitmap> item = this.game.Graphics.Screens[i];

								if (item.Value.Visible)
								{
									int iBitmapAddress0 = ((iRow * this.screenSize.Height) * iStride) +
										((iColumn * this.screenSize.Width) * iPixelByteSize);

									GBitmap bitmap = item.Value;
									int iBitmapAddress = iBitmapAddress0;
									int iGBitmapAddress0 = 0;

									for (int y = 0; y < bitmap.Height; y++)
									{
										int iGBitmapAddress = iGBitmapAddress0;
										iBitmapAddress = iBitmapAddress0;

										for (int x = 0; x < bitmap.Width; x++)
										{
											Color color = bitmap.Palette[bitmap.Pixels[iGBitmapAddress]];

											Marshal.WriteInt32(l.Address, iBitmapAddress, (int)color.ToUInt32());
											iBitmapAddress += iPixelByteSize;

											iGBitmapAddress++;
										}

										iBitmapAddress0 += iStride;
										iGBitmapAddress0 += bitmap.Width;
									}

									item.Value.Modified = false;

									iColumn++;
									if (iColumn >= this.screenColumns)
									{
										iColumn = 0;
										iRow++;
									}
								}
							}
						}

						this.mainImage.Source = currentBitmap;

						this.bitmapIndex++;
						this.bitmapIndex %= 2;
					}
				}
			}
		}

		private void GameThread()
		{
			bool savePreferences = true;

			try
			{
				this.game.Start();
			}
			catch (ApplicationExitException)
			{
			}
			catch (ResourceMissingException ex)
			{
				// Show exceptions on UI Thread
				this.gameException = ex;
				savePreferences = false;
			}
#if !DEBUG
			catch (Exception ex)
			{
				// Show exceptions on UI Thread
				this.gameException = ex;
				savePreferences = false;
			}
#endif

			if (savePreferences)
			{
				this.game.GlobalPreferences.SerializeToJSON(this.game, this.game.MainPath);
			}
		}
	}
}
