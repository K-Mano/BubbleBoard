
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Windows.Interop
Imports System.Windows.Media.Animation
<StructLayout(LayoutKind.Sequential)>
Friend Structure WindowCompositionAttributeData
    Public Attribute As WindowCompositionAttribute
    Public Data As IntPtr
    Public SizeOfData As Integer
End Structure

Friend Enum WindowCompositionAttribute
    WCA_ACCENT_POLICY = 19
End Enum

Friend Enum AccentState
    ACCENT_DISABLED = 0
    ACCENT_ENABLE_GRADIENT = 1
    ACCENT_ENABLE_TRANSPARENTGRADIENT = 2
    ACCENT_ENABLE_BLURBEHIND = 3
    ACCENT_INVALID_STATE = 4
End Enum

<StructLayout(LayoutKind.Sequential)>
Friend Structure AccentPolicy
    Public AccentState As AccentState
    Public AccentFlags As Integer
    Public GradientColor As UInteger
    Public AnimationId As Integer
End Structure


Partial Public Class MainWindow
    Inherits Window
    Public Sub New()
            InitializeComponent()
        End Sub

        Public Overrides Sub OnApplyTemplate()
            MyBase.OnApplyTemplate()
            EnableBlur(Me)
        End Sub

    <DllImport("user32.dll")>
    Friend Shared Function SetWindowCompositionAttribute(ByVal hwnd As IntPtr, ByRef data As WindowCompositionAttributeData) As Integer

    End Function

    Friend Shared Sub EnableBlur(ByVal win As Window)
        Dim windowHelper = New WindowInteropHelper(win)
        Dim accent = New AccentPolicy()
        Dim accentStructSize = Marshal.SizeOf(accent)
        accent.AccentState = AccentState.ACCENT_ENABLE_BLURBEHIND
        accent.AccentFlags = 2
        accent.GradientColor = &H44FFFFBBUI
        Dim accentPtr = Marshal.AllocHGlobal(accentStructSize)
        Marshal.StructureToPtr(accent, accentPtr, False)
        Dim data = New WindowCompositionAttributeData With {
            .Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY,
            .SizeOfData = accentStructSize,
            .Data = accentPtr
        }
        SetWindowCompositionAttribute(windowHelper.Handle, data)
        Marshal.FreeHGlobal(accentPtr)
    End Sub
    Private Sub Window_Loaded(ByVal sender As Object, ByVal e As RoutedEventArgs) Handles Me.Loaded
        NotiViewBackground.Visibility = Visibility.Hidden
        EnableBlur(Me)
    End Sub

    Private Sub ApplyButton_Clicked(sender As Object, e As RoutedEventArgs) Handles ApplyButton.Click
        If TitleBox.Text.Length > 0 And LinkBox.Text.Length > 0 Then
            If LinkBox.Text.Contains("https://") Or LinkBox.Text.Contains("http://") Then
                Try
#Disable Warning BC42025 ' インスタンスを経由する共有メンバー、定数メンバー、列挙型メンバー、または入れ子にされた型へのアクセスです
                    If Directory.Exists(".\WorkDirs") Then

                    Else
                        Directory.CreateDirectory(".\WorkDirs")
                    End If
                    Directory.CreateDirectory(".\WorkDirs\" + TitleBox.Text)
                    Dim shortcutPath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), TitleBox.Text + ".lnk")
                    Dim targetPath As String = Path.GetDirectoryName(Reflection.Assembly.GetExecutingAssembly().Location) + "\Resources\Bubbles.exe"
                    Dim t As Type = Type.GetTypeFromCLSID(New Guid("72C24DD5-D70A-438B-8A42-98424B88AFB8"))
                    Dim shell = Activator.CreateInstance(t)
                    Dim shortcut = shell.CreateShortcut(shortcutPath)
                    shortcut.TargetPath = targetPath
                    shortcut.Arguments = LinkBox.Text
                    shortcut.WorkingDirectory = Path.GetDirectoryName(Reflection.Assembly.GetExecutingAssembly().Location) + "\WorkDirs\" + TitleBox.Text
                    'shortcut.IconLocation = Application.ExecutablePath + ",0"
                    shortcut.Save()
                    Marshal.FinalReleaseComObject(shortcut)
                    Marshal.FinalReleaseComObject(shell)
                    TitleText.Text = "情報"
                    Description.Text = "作成に成功しました。"
                    NotiViewAnimationStartup()
                Catch ex As Exception
                    TitleText.Text = "ファイル操作"
                    Description.Text = "ショートカットの作成に失敗しました。"
                    NotiViewAnimationStartup()
                End Try
            Else
                TitleText.Text = "作成失敗"
                Description.Text = "リンクには""https://""または""http://""を含む必要があります。"
                NotiViewAnimationStartup()
            End If
        Else
            NotiViewBackground.Opacity = 0
            TitleText.Text = "作成失敗"
            Description.Text = "タイトルとリンクを入力してください。"
            NotiViewAnimationStartup()
        End If
    End Sub

    Private Sub GlassLec_LeftDown(sender As Object, e As MouseButtonEventArgs) Handles GlassBorder.MouseLeftButtonDown
        If e.ButtonState <> MouseButtonState.Pressed Then Return
        DragMove()
    End Sub
    Private Sub NotiViewAnimationStartup()
        NotiViewBackground.Visibility = Visibility.Visible
        NotiViewBackground.BeginAnimation(OpacityProperty, New DoubleAnimation(1, TimeSpan.FromSeconds(0.2)))
        BlurEffect.BeginAnimation(BlurEffect.RadiusProperty, animation:=New DoubleAnimation(30, TimeSpan.FromSeconds(0.2)))
    End Sub
    Private Async Sub NotiViewAnimationShutDown()
        NotiViewBackground.BeginAnimation(OpacityProperty, New DoubleAnimation(0, TimeSpan.FromSeconds(0.2)))
        BlurEffect.BeginAnimation(BlurEffect.RadiusProperty, New DoubleAnimation(0, TimeSpan.FromSeconds(0.2)))
        Await Task.Delay(200)
        NotiViewBackground.Visibility = Visibility.Hidden
    End Sub
    Private Sub OKButton_Click(sender As Object, e As RoutedEventArgs) Handles OKButton.Click
        NotiViewAnimationShutDown()
    End Sub

    Private Sub NotiView_LeftDown(sender As Object, e As MouseButtonEventArgs) Handles NotiViewBackground.MouseLeftButtonDown
        If e.ButtonState <> MouseButtonState.Pressed Then Return
        DragMove()
    End Sub

    Private Function ConvertHexToColor(hex As String) As Color
        Dim color As Object = ColorConverter.ConvertFromString(hex)
        Return color
    End Function

    Private Sub OKButton_MouseEnter(sender As Object, e As MouseEventArgs) Handles OKButton.MouseEnter
        OKButtonText.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, New ColorAnimation(ConvertHexToColor("#FF464646"), TimeSpan.FromSeconds(0.1)))
    End Sub

    Private Sub OKButton_MouseLeave(sender As Object, e As MouseEventArgs) Handles OKButton.MouseLeave
        OKButtonText.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, New ColorAnimation(ConvertHexToColor("#FFFFFFFF"), TimeSpan.FromSeconds(0.1)))
    End Sub
End Class

