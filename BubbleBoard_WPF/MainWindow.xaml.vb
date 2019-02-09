
Imports System.IO
Imports System.Text

Namespace MainWindow
    Class MainWindow
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
    End Class
End Namespace

