Imports System.Drawing
Imports System.Drawing.Imaging








Public Class textInBitmap
 

    Public Shared Sub TextOnImage(ByVal OldImage As String, ByVal NewImage As String, ByVal Text As String,
                                  ByVal Format As ImageFormat, ByVal Font As Font, ByVal Color As Color, ByVal Position As Point)
        Dim TmpSize As System.Drawing.Size
        Dim Image As Image = System.Drawing.Image.FromFile(OldImage)
        Dim Brush As New SolidBrush(Color)

        'Read Image Dimensions
        TmpSize.Height = Image.Height
        TmpSize.Width = Image.Width

        'Create a new Bitmap Object
        Dim NewBitmap As New System.Drawing.Bitmap(Image, TmpSize)

        'Create a new Graphic Object
        Dim Graphic As System.Drawing.Graphics = System.Drawing.Graphics.FromImage(NewBitmap)

        'Draw String on Image
        Graphic.DrawString(Text, Font, Brush, Position)

        'Save new Image
        NewBitmap.Save(NewImage, Format)

        Graphic.Dispose()
        NewBitmap.Dispose()
    End Sub

End Class
