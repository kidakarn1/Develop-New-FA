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
Public Class ins_time_loss
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

		Loss_reg_pass.Enabled = True

		Me.Hide()
	End Sub
	Private Sub Button10_Click(sender As Object, e As EventArgs) Handles Button10.Click
		Try
			If My.Computer.Network.Ping("192.168.161.101") Then
				Try
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
					Dim date_cerrunt As Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
					Dim time_cerrunt As Date = DateTime.Now.ToString("HH:mm:ss")
					If TextBox1.Text >= time_cerrunt Then
						start_time = date_cerrunt.AddDays(-1)
						Dim s As String = start_time.ToString().Substring(0, 9)
						start_time = s & TextBox1.Text
						start_time = Convert.ToDateTime(start_time).ToString("yyyy-MM-dd HH:mm:ss")
					End If
					If TextBox2.Text > time_cerrunt Then
						end_time = date_cerrunt.AddDays(-1)
						Dim s As String = end_time.ToString().Substring(0, 9)
						end_time = s & TextBox2.Text
						end_time = Convert.ToDateTime(end_time).ToString("yyyy-MM-dd HH:mm:ss")
					End If
					Dim GET_CHECK_LOSS_reuslt = Backoffice_model.GET_CHECK_LOSS(start_time, end_time)
					Dim check_double_loss As Integer = 0
					Dim count_check_double As String = ""
					count_check_double = GET_CHECK_LOSS_reuslt
					If CDbl(Val(count_check_double)) > 0 Then
						MsgBox("Loss double please check start loss and end loss")
					Else
						Dim time_st As Date = Date.Parse(Label2.Text)
						Dim time_now As Date = Date.Now
						Dim Minutes_total As Long = DateDiff(DateInterval.Minute, time_now, time_st)
						Minutes_total = Math.Abs(Minutes_total)
						If total_time_loss > Minutes_total Or total_time_loss = 0 Then
							MsgBox("Time insert is incorrected. Please try again.")
						Else
							'MsgBox("Corrected")
							Loss_reg_pass.Label8.Text = TextBox1.Text
							Loss_reg_pass.Label9.Text = TextBox2.Text
							Loss_reg_pass.Enabled = True
							Loss_reg_pass.Button1.Visible = True
							Me.Hide()
						End If
					End If
				Catch ex As Exception
					MsgBox("Please check time ")
				End Try
			Else
				load_show.Show()
			End If
		Catch ex As Exception
			load_show.Show()
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
		If TextBox1.Text.Length = 1 Then
			If TextBox1.Text = "0" Or TextBox1.Text = "1" Then
				allena()
			Else
				ena0123()
			End If
		ElseIf TextBox1.Text.Length = 2 Then
			ena012345()
		ElseIf TextBox1.Text.Length = 4 Then
			allena()
		ElseIf TextBox1.Text.Length = 5 Then
			ena012()
		End If

		If TextBox2.Text.Length = 1 Then
			If TextBox2.Text = "0" Or TextBox2.Text = "1" Then
				allena()
			Else
				ena0123()
			End If
		ElseIf TextBox2.Text.Length = 2 Then
			ena012345()
		ElseIf TextBox2.Text.Length = 4 Then
			allena()
		ElseIf TextBox2.Text.Length = 5 Then
			ena012()
		End If

		If TextBox1.Text.Length = 0 Then
			ena012()
		End If
	End Sub

	Private Sub Label2_Click(sender As Object, e As EventArgs) Handles Label2.Click

	End Sub


End Class
