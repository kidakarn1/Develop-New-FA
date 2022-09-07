Imports VB = Microsoft.VisualBasic
Imports System
Imports System.Management
Imports System.ComponentModel
Imports System.Threading
Imports System.IO
Imports System.IO.Ports
Imports System.Text
Imports System.Globalization
Imports System.Drawing.Imaging
Imports IDAutomation.Windows.Forms.LinearBarCode
Imports System.Drawing.Printing
Imports System.Configuration
Imports GenCode128
Imports BarcodeLib.Barcode
Public Class ins_qty_special_time
	Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
		Dim text_now As String = TextBox1.Text
		If text_now.Length > 4 Then
			Dim text_to2 As String = TextBox2.Text
			If text_to2.Length = 2 Then
				text_to2 = text_to2 & ":1"
				TextBox2.Text = text_to2
			ElseIf text_to2.Length = 1 Then
				text_to2 = text_to2 & "1"
				TextBox2.Text = text_to2
				allena()
			Else
				text_to2 = text_to2 & "1"
				TextBox2.Text = text_to2
			End If
		Else
			If text_now.Length = 2 Then
				text_now = text_now & ":1"
				TextBox1.Text = text_now
			Else
				text_now = text_now & "1"
				TextBox1.Text = text_now
			End If
		End If
		chk_ins()
		Dim text_to2_now As String = TextBox2.Text
		If text_to2_now.Length > 4 Then
			alldis()
		End If
	End Sub
	Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
		Dim text_now As String = TextBox1.Text
		If text_now.Length > 4 Then
			Dim text_to2 As String = TextBox2.Text
			If text_to2.Length = 2 Then
				text_to2 = text_to2 & ":2"
				TextBox2.Text = text_to2
			Else
				text_to2 = text_to2 & "2"
				TextBox2.Text = text_to2
			End If
		Else
			If text_now.Length = 2 Then
				text_now = text_now & ":2"
				TextBox1.Text = text_now
			Else
				text_now = text_now & "2"
				TextBox1.Text = text_now
			End If
		End If
		chk_ins()
		Dim text_to2_now As String = TextBox2.Text
		If text_to2_now.Length > 4 Then
			alldis()
		End If
	End Sub
	Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
		Dim text_now As String = TextBox1.Text
		If text_now.Length > 4 Then
			Dim text_to2 As String = TextBox2.Text

			If text_to2.Length = 2 Then
				text_to2 = text_to2 & ":3"
				TextBox2.Text = text_to2
			Else
				text_to2 = text_to2 & "3"
				TextBox2.Text = text_to2
			End If
		Else
			If text_now.Length = 2 Then
				text_now = text_now & ":3"
				TextBox1.Text = text_now
			Else
				text_now = text_now & "3"
				TextBox1.Text = text_now
			End If
		End If
		chk_ins()
		Dim text_to2_now As String = TextBox2.Text
		If text_to2_now.Length > 4 Then
			alldis()
		End If
	End Sub
	Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
		Dim text_now As String = TextBox1.Text
		If text_now.Length > 4 Then
			Dim text_to2 As String = TextBox2.Text
			If text_to2.Length = 2 Then
				text_to2 = text_to2 & ":4"
				TextBox2.Text = text_to2
			Else
				text_to2 = text_to2 & "4"
				TextBox2.Text = text_to2
			End If
		Else
			If text_now.Length = 2 Then
				text_now = text_now & ":4"
				TextBox1.Text = text_now
			Else
				text_now = text_now & "4"
				TextBox1.Text = text_now
			End If
		End If
		chk_ins()
		Dim text_to2_now As String = TextBox2.Text
		If text_to2_now.Length > 4 Then
			alldis()
		End If
	End Sub
	Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
		Dim text_now As String = TextBox1.Text
		If text_now.Length > 4 Then
			Dim text_to2 As String = TextBox2.Text
			If text_to2.Length = 2 Then
				text_to2 = text_to2 & ":5"
				TextBox2.Text = text_to2
			Else
				text_to2 = text_to2 & "5"
				TextBox2.Text = text_to2
			End If
		Else
			If text_now.Length = 2 Then
				text_now = text_now & ":5"
				TextBox1.Text = text_now
			Else
				text_now = text_now & "5"
				TextBox1.Text = text_now
			End If
		End If
		chk_ins()
		Dim text_to2_now As String = TextBox2.Text
		If text_to2_now.Length > 4 Then
			alldis()
		End If
	End Sub
	Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
		Dim text_now As String = TextBox1.Text
		If text_now.Length > 4 Then
			Dim text_to2 As String = TextBox2.Text
			If text_to2.Length = 2 Then
				text_to2 = text_to2 & ":6"
				TextBox2.Text = text_to2
			Else
				text_to2 = text_to2 & "6"
				TextBox2.Text = text_to2
			End If
		Else
			If text_now.Length = 2 Then
				text_now = text_now & ":6"
				TextBox1.Text = text_now
			Else
				text_now = text_now & "6"
				TextBox1.Text = text_now
			End If
		End If
		chk_ins()
		Dim text_to2_now As String = TextBox2.Text
		If text_to2_now.Length > 4 Then
			alldis()
		End If
	End Sub
	Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
		Dim text_now As String = TextBox1.Text
		If text_now.Length > 4 Then
			Dim text_to2 As String = TextBox2.Text

			If text_to2.Length = 2 Then
				text_to2 = text_to2 & ":7"
				TextBox2.Text = text_to2
			Else
				text_to2 = text_to2 & "7"
				TextBox2.Text = text_to2
			End If

		Else
			If text_now.Length = 2 Then
				text_now = text_now & ":7"
				TextBox1.Text = text_now
			Else
				text_now = text_now & "7"
				TextBox1.Text = text_now
			End If
		End If
		chk_ins()
		Dim text_to2_now As String = TextBox2.Text
		If text_to2_now.Length > 4 Then
			alldis()

		End If
	End Sub

	Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
		Dim text_now As String = TextBox1.Text
		If text_now.Length > 4 Then
			Dim text_to2 As String = TextBox2.Text

			If text_to2.Length = 2 Then
				text_to2 = text_to2 & ":8"
				TextBox2.Text = text_to2
			Else
				text_to2 = text_to2 & "8"
				TextBox2.Text = text_to2
			End If
		Else
			If text_now.Length = 2 Then
				text_now = text_now & ":8"
				TextBox1.Text = text_now
			Else
				text_now = text_now & "8"
				TextBox1.Text = text_now
			End If
		End If
		chk_ins()
		Dim text_to2_now As String = TextBox2.Text
		If text_to2_now.Length > 4 Then
			alldis()
		End If
	End Sub
	Private Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click
		Dim text_now As String = TextBox1.Text
		If text_now.Length > 4 Then
			Dim text_to2 As String = TextBox2.Text
			If text_to2.Length = 2 Then
				text_to2 = text_to2 & ":9"
				TextBox2.Text = text_to2
			Else
				text_to2 = text_to2 & "9"
				TextBox2.Text = text_to2
			End If
		Else
			If text_now.Length = 2 Then
				text_now = text_now & ":9"
				TextBox1.Text = text_now
			Else
				text_now = text_now & "9"
				TextBox1.Text = text_now
			End If
		End If
		chk_ins()
		Dim text_to2_now As String = TextBox2.Text
		If text_to2_now.Length > 4 Then
			alldis()
		End If
	End Sub
	Private Sub Button13_Click(sender As Object, e As EventArgs) Handles Button13.Click
		Dim text_now As String = TextBox1.Text
		If text_now.Length > 4 Then
			Dim text_to2 As String = TextBox2.Text

			If text_to2.Length = 2 Then
				text_to2 = text_to2 & ":0"
				TextBox2.Text = text_to2
			Else
				text_to2 = text_to2 & "0"
				TextBox2.Text = text_to2
			End If
		Else
			If text_now.Length = 2 Then
				text_now = text_now & ":0"
				TextBox1.Text = text_now
			Else
				text_now = text_now & "0"
				TextBox1.Text = text_now
			End If
		End If
		chk_ins()
		Dim text_to2_now As String = TextBox2.Text
		If text_to2_now.Length > 4 Then
			alldis()
		End If
	End Sub

	Private Sub Button12_Click(sender As Object, e As EventArgs) Handles Button12.Click
		TextBox1.Clear()
		TextBox2.Clear()
		ena012()
		'Button1.Enabled = True
		'Button2.Enabled = True
		'Button3.Enabled = True
		'Button4.Enabled = True
		'Button5.Enabled = True
		'Button6.Enabled = True
		'Button7.Enabled = True
		'Button8.Enabled = True
		'Button9.Enabled = True
		'Button13.Enabled = True

	End Sub

	Private Sub Button14_Click(sender As Object, e As EventArgs) Handles Button14.Click
		Dim text_to2 As String = TextBox2.Text
		Dim text_to1 As String = TextBox1.Text
		If text_to2.Length > 0 Then
			'MsgBox(text_to2.Length)
			If text_to2.Length = 1 Then
				Dim result_str As String = text_to2.Substring(1)
				TextBox2.Text = result_str
			ElseIf text_to2.Length = 2 Then
				Dim result_str As String = text_to2.Substring(0, 1)
				TextBox2.Text = result_str
			ElseIf text_to2.Length = 4 Then
				Dim result_str As String = text_to2.Substring(0, 2)
				TextBox2.Text = result_str
			ElseIf text_to2.Length = 5 Then
				Dim result_str As String = text_to2.Substring(0, 4)
				TextBox2.Text = result_str
			End If
		End If

		If text_to2.Length = 0 Then
			If text_to1.Length > 0 Then
				'MsgBox(text_to2.Length)
				If text_to1.Length = 1 Then
					Dim result_str As String = text_to1.Substring(1)
					TextBox1.Text = result_str
				ElseIf text_to1.Length = 2 Then
					Dim result_str As String = text_to1.Substring(0, 1)
					TextBox1.Text = result_str
				ElseIf text_to1.Length = 4 Then
					Dim result_str As String = text_to1.Substring(0, 2)
					TextBox1.Text = result_str
				ElseIf text_to1.Length = 5 Then
					Dim result_str As String = text_to1.Substring(0, 4)
					TextBox1.Text = result_str
				End If
			End If
		End If

		'text_to2 = TextBox2.Text
		'MsgBox(TextBox2.Text.Length)
		chk_ins()
	End Sub

	Private Sub Button11_Click(sender As Object, e As EventArgs) Handles Button11.Click
		TextBox1.Clear()
		TextBox2.Clear()
		ena012()
		Me.Close()
	End Sub

	Private Sub Button10_Click(sender As Object, e As EventArgs) Handles Button10.Click
		Try
			Dim date_data As Date = DateTime.Now.ToString("HH:mm:ss")
			If (TextBox1.Text >= Backoffice_model.start_master_shift.Substring(0, 5) And TextBox1.Text <= Backoffice_model.coles_lot_end_shift.Substring(0, 5)) Then
				If (TextBox2.Text >= Backoffice_model.start_master_shift.Substring(0, 5) And TextBox2.Text <= Backoffice_model.coles_lot_end_shift.Substring(0, 5)) Then
					Dim total_time_loss As Integer
					Dim date1 As Date = Date.Parse(TextBox1.Text)
					Dim date2 As Date = Date.Parse(TextBox2.Text)
					Try
						Dim Minutes As Long = DateDiff(DateInterval.Minute, date1, date2)
						If Minutes.ToString < 0 Then
							Dim temmpola As String = Minutes.ToString.Substring(0)
							Dim tempp As Integer = temmpola
							tempp = Math.Abs(tempp)
							total_time_loss = 1440 - tempp
						Else
							total_time_loss = Minutes.ToString
						End If
					Catch ex As Exception
					End Try
					Dim start_time As String = date1.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
					Dim end_time As String = date2.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
					Dim date_now As String = date1.ToString("yyyy-MM-dd ", CultureInfo.InvariantCulture)
					Dim check_end As String = TextBox2.Text
					Dim now_time As Date = DateTime.Now.ToString("H:m:s")
					Dim end_check_date_paralell_line As String = ""
					Dim start_check_date_paralell_line As String = ""
					If TextBox1.Text < TextBox2.Text Then
						If TextBox2.Text >= "00:00" And TextBox2.Text <= "07:59" Then
							Dim date_cerrunt As Date = DateTime.Now.ToString("yyyy-MM-dd")
							Dim result_date_end As Date = date_cerrunt.AddDays(1) & " " & TextBox2.Text
							end_check_date_paralell_line = result_date_end.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
						Else
							end_check_date_paralell_line = end_time
						End If
						If TextBox1.Text >= "00:00" And TextBox1.Text <= "07:59" Then
							Dim date_cerrunt As Date = DateTime.Now.ToString("yyyy-MM-dd")
							Dim result_date_start As Date = date_cerrunt.AddDays(1) & " " & TextBox1.Text
							start_check_date_paralell_line = result_date_start.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
						Else
							start_check_date_paralell_line = start_time
						End If
						Dim check_time = Backoffice_model.check_time(Working_Pro.Label22.Text, Working_Pro.wi_no.Text, start_check_date_paralell_line, end_check_date_paralell_line)
						If check_time = "0" Then
							Backoffice_model.start_check_date_paralell_line = start_check_date_paralell_line
							Backoffice_model.end_check_date_paralell_line = end_check_date_paralell_line
							Me.Close()
						Else
							MsgBox("TIME DOUBLE")
						End If
					Else
						MsgBox("Please check Time")
					End If
				Else
					MsgBox("Please check Time")
				End If
			Else
				MsgBox("Please check shift")
			End If
		Catch ex As Exception
			MsgBox("Please check Time")
			'MsgBox("ex.message = " & ex.Message)
		End Try

	End Sub
	Private Sub ena012()
		Button1.Enabled = True
		Button2.Enabled = True
		Button3.Enabled = False
		Button4.Enabled = False
		Button5.Enabled = False
		Button6.Enabled = False
		Button7.Enabled = False
		Button8.Enabled = False
		Button9.Enabled = False
		Button13.Enabled = True
	End Sub
	Private Sub ena0123()
		Button1.Enabled = True
		Button2.Enabled = True
		Button3.Enabled = True
		Button4.Enabled = False
		Button5.Enabled = False
		Button6.Enabled = False
		Button7.Enabled = False
		Button8.Enabled = False
		Button9.Enabled = False
		Button13.Enabled = True
	End Sub
	Private Sub ena012345()
		Button1.Enabled = True
		Button2.Enabled = True
		Button3.Enabled = True
		Button4.Enabled = True
		Button5.Enabled = True
		Button6.Enabled = False
		Button7.Enabled = False
		Button8.Enabled = False
		Button9.Enabled = False
		Button13.Enabled = True
	End Sub
	Private Sub alldis()
		Button1.Enabled = False
		Button2.Enabled = False
		Button3.Enabled = False
		Button4.Enabled = False
		Button5.Enabled = False
		Button6.Enabled = False
		Button7.Enabled = False
		Button8.Enabled = False
		Button9.Enabled = False
		Button13.Enabled = False

		Button10.Enabled = True
	End Sub

	Private Sub allena()
		Button1.Enabled = True
		Button2.Enabled = True
		Button3.Enabled = True
		Button4.Enabled = True
		Button5.Enabled = True
		Button6.Enabled = True
		Button7.Enabled = True
		Button8.Enabled = True
		Button9.Enabled = True
		Button13.Enabled = True
	End Sub

	Private Sub chk_ins()
		Button1.Enabled = True
		Button2.Enabled = True
		Button3.Enabled = True
		Button4.Enabled = True
		Button5.Enabled = True
		Button6.Enabled = True
		Button7.Enabled = True
		Button8.Enabled = True
		Button9.Enabled = True
		Button13.Enabled = True
	End Sub

	Private Sub ins_qty_special_time_Load(sender As Object, e As EventArgs) Handles MyBase.Load
		chk_ins()
	End Sub
End Class
