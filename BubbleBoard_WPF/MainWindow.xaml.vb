
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Windows.Interop

Namespace MainWindow
    Class MainWindow

        <DllImport("user32.dll")>
        Friend Shared Function SetWindowCompositionAttribute(ByVal hwnd As IntPtr, ByRef data As WindowCompositionAttributeData) As Integer

        End Function

        Private Sub ApplyButton_Clicked(sender As Object, e As RoutedEventArgs) Handles ApplyButton.Click
            If TitleBox.Text.Length > 0 And LinkBox.Text.Length > 0 Then
                Try
                    If Directory.Exists(".\bubbles") Then

                    Else
                        Directory.CreateDirectory(".\bubbles")
                    End If
                    Directory.CreateDirectory(".\bubbles\Bubble_" + TitleBox.Text)
                    File.Copy(".\Bubbles.exe", ".\bubbles\Bubble_" + TitleBox.Text + "\Bubble_" + TitleBox.Text + ".exe", True)
                    Dim setlink As New StreamWriter(".\bubbles\Bubble_" + TitleBox.Text + "\Bubble_" + TitleBox.Text + ".dat", False, Encoding.GetEncoding("shift_jis"))
                    setlink.Write(LinkBox.Text)
                    setlink.Close()
                    MessageBox.Show("Creation Succeed.", "Information", MessageBoxButton.OK, MessageBoxImage.Information)
                Catch ex As Exception
                    MessageBox.Show("Failed to Copying Files.", "Error", MessageBoxButton.OK, MessageBoxImage.Error)
                End Try
            Else
                MessageBox.Show("Input the Title and Link.", "Error", MessageBoxButton.OK, MessageBoxImage.Error)
            End If
        End Sub

        Private Sub GlassLec_LeftDown(sender As Object, e As MouseButtonEventArgs) Handles GlassBorder.MouseLeftButtonDown
            If e.ButtonState <> MouseButtonState.Pressed Then Return
            DragMove()
        End Sub

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
    End Class
    Partial Public Class MainWindow
        Inherits Window

        Public Overrides Sub OnApplyTemplate()
            MyBase.OnApplyTemplate()
            EnableBlur(Me)
        End Sub

        Friend Shared Sub EnableBlur(ByVal win As Window)
            Dim windowHelper = New WindowInteropHelper(win)
            Dim accent = New AccentPolicy()
            Dim accentStructSize = Marshal.SizeOf(accent)
            accent.AccentState = AccentState.ACCENT_ENABLE_BLURBEHIND
            accent.AccentFlags = 2
            accent.GradientColor = &H99222222UI
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
    End Class
End Namespace

