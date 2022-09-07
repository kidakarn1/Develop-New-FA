Imports System.Net
Imports System.Web.Script.Serialization
Public Class Line_conf
	Private Sub Label11_Click(sender As Object, e As EventArgs) Handles Label11.Click

	End Sub

	Private Sub Label5_Click(sender As Object, e As EventArgs) Handles Label5.Click

	End Sub

	Private Sub Label3_Click(sender As Object, e As EventArgs)

	End Sub

	Private Sub Label9_Click(sender As Object, e As EventArgs) Handles Label9.Click

	End Sub

	Private Sub Label6_Click(sender As Object, e As EventArgs)

	End Sub

	Private Sub Timer2_Tick(sender As Object, e As EventArgs) Handles Timer2.Tick
		Label1.Text = TimeOfDay.ToString("H:mm:ss")
		Label22.Text = DateTime.Now.ToString("yyyy/MM/dd")
	End Sub

	Private Sub Label1_Click(sender As Object, e As EventArgs) Handles Label1.Click

	End Sub

	Private Sub Line_conf_Load(sender As Object, e As EventArgs) Handles MyBase.Load
		Dim api = New api()
		Timer2.Start()
		'ComboBox1.SelectedIndex = ComboBox1.FindString("DEFAULT")
		ComboBox3.SelectedIndex = ComboBox3.FindString("K1PL00")
		ComboBox1.SelectedIndex = ComboBox1.FindString("COM1")
		ComboBox5.SelectedIndex = ComboBox5.FindString("BUTTON")
		ComboBox4.SelectedIndex = ComboBox4.FindString("1")
		ComboBox6.SelectedIndex = ComboBox6.FindString("USB")
		ComboBox7.SelectedIndex = ComboBox7.FindString("DIO000")
		Load_PD()
		Load_Line()
		'ComboBox2.DataSource = GetLineItems()
		'ComboBox2.DisplayMember = "Name"
		'ComboBox2.ValueMember = "ID"
		' LoadSQL = Backoffice_model.GetLine_mst()
		'While LoadSQL.Read()
		'Label6.Text = LoadSQL("pd").ToString()
		'Label4.Text = LoadSQL("line_cd").ToString()
		'MsgBox(LoadSQL("dep_cd").ToString())
		'End While
	End Sub
	Private Sub Load_PD()
		ComboBox3.Items.Clear()
		Dim api = New api()
		Dim BF = New Backoffice_model()
		Dim I_PD_CD As Integer = 0
		Dim check_I_PD As Integer = 0
		Dim get_default_pd = BF.Get_default_pd_detail()
		Dim PD As String = ""
		Dim result_data As String = api.Load_data("http://192.168.161.207/API_NEW_FA/GET_DATA_NEW_FA/GET_PD")
		While get_default_pd.Read()
			PD = get_default_pd("pd").ToString()
		End While
		If result_data <> "0" Then
			Dim dict2 As Object = New JavaScriptSerializer().Deserialize(Of List(Of Object))(result_data)
			For Each item As Object In dict2
				ComboBox3.Items.Add(item("PD").ToString())
				If item("PD").ToString() = PD Then
					check_I_PD = I_PD_CD
				End If
				I_PD_CD += 1
			Next
			ComboBox3.SelectedIndex = check_I_PD
		End If
	End Sub
	Private Sub Load_Line()
		ComboBox2.Items.Clear()
		Dim api = New api()
		Dim BF = New Backoffice_model()
		Dim result_data = api.Load_data("http://192.168.161.207/API_NEW_FA/GET_DATA_NEW_FA/GET_LINE?PD=" & ComboBox3.Text)
		Dim PD As String = "NO_DATA"
		Dim LINE_CD As String = "NO_DATA"
		Dim I_PD As Integer = 0
		Dim check_I_LINE As Integer = 0
		Dim I_LINE_CD As Integer = 0
		Dim get_default_line = BF.Get_default_line_detail()
		While get_default_line.Read()
			PD = get_default_line("pd").ToString()
			LINE_CD = get_default_line("line_cd").ToString()
		End While
		If result_data <> "0" Then
			Dim dict2 As Object = New JavaScriptSerializer().Deserialize(Of List(Of Object))(result_data)
			For Each item As Object In dict2
				ComboBox2.Items.Add(item("LINE_CD").ToString())
				If item("LINE_CD").ToString() = LINE_CD Then
					check_I_LINE = I_LINE_CD
				End If
				I_LINE_CD += 1
			Next
			ComboBox2.SelectedIndex = check_I_LINE
		End If
	End Sub

	Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
		Dim LoadSQL As String
		Dim pd As String = ComboBox3.Items(ComboBox3.SelectedIndex)
		Dim line_cd As String = ComboBox2.Text
		Dim count_type As String = ComboBox5.Text
		Dim cavity As String = ComboBox4.Text
		Dim scanner_port As String = ComboBox1.Text
		Dim printer_port As String = ComboBox6.Text
		Dim dio_port As String = ComboBox7.Text

		Backoffice_model.saveLineConfig(pd, line_cd, count_type, cavity, scanner_port, printer_port, dio_port)
		'Dim f2 = New MainFrm()
		Dim sqlss = Backoffice_model.ConnectDBSQLite()
		While sqlss.Read()
			MainFrm.Label6.Text = sqlss("pd").ToString()
			MainFrm.Label4.Text = sqlss("line_cd").ToString()
			MainFrm.count_type.Text = sqlss("count_type").ToString()
			MainFrm.cavity.Text = sqlss("cavity").ToString()
			MainFrm.lb_scanner_port.Text = sqlss("scanner_port").ToString()
			MainFrm.lb_printer_port.Text = sqlss("printer_port").ToString()
			MainFrm.lb_dio_port.Text = sqlss("dio_port").ToString()
			Backoffice_model.SCANNER_PORT = sqlss("scanner_port").ToString()
			'MsgBox(sqlss("pd").ToString())
		End While
		If Backoffice_model.SCANNER_PORT <> "" And Backoffice_model.SCANNER_PORT <> "USB" Then
			MainFrm.lb_ctrl_sc_flg.Text = "emp"
		End If
		Insert_list.Label3.Text = MainFrm.Label4.Text
		Prd_detail.Label3.Text = MainFrm.Label4.Text
		MainFrm.Show()
		Me.Hide()
	End Sub
	Private Sub menu3_Click(sender As Object, e As EventArgs) Handles menu3.Click
		Me.Enabled = False
		Me.ComboBox2.DataSource = Nothing
		Try
			If My.Computer.Network.Ping("192.168.161.102") Then
				Dim webClient As New System.Net.WebClient
				Dim result As String = webClient.DownloadString("http://192.168.161.102/exp_api3party/Api_sync_newfa/update_line_mst")
				MsgBox("Synchronous completed")
			Else
				MsgBox("Synchronous not completed")
			End If
		Catch ex As Exception

		End Try
		'Me.ComboBox2.DataSource = GetLineItems()
		'Me.ComboBox2.DisplayMember = "Name"
		'Me.ComboBox2.ValueMember = "ID"
		Dim pd As String = ComboBox3.Items(ComboBox3.SelectedIndex)
		Dim line_cd As String = ComboBox2.Text
		Me.Enabled = True


		'Dim f2 = New Line_conf()
		'Me.Close()
		'Me.Show()
	End Sub

	Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs)

	End Sub
	Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
		'Dim f2 = New MainFrm()

		MainFrm.Show()
		Me.Close()
	End Sub
	Private Sub ComboBox2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox2.SelectedIndexChanged

	End Sub
	Function GetLineItems() As List(Of LineItem)
		Dim LineItems = New List(Of LineItem)
		'Command = New MySqlCommand("SELECT * FROM `Linelist` WHERE l_id = '" & id & "'", connection)
		'Command.CommandTimeout = 30
		'Reader = Command.ExecuteReader()
		Dim LoadSQL = Backoffice_model.GetLine_mst()
		'MsgBox("test")
		' While LoadSQL.Read()
		'Label6.Text = LoadSQL("pd").ToString()
		'Label4.Text = LoadSQL("line_cd").ToString()
		'MsgBox(LoadSQL("dep_cd").ToString())
		'End While
		If LoadSQL.HasRows = True Then
			While LoadSQL.Read()
				LineItems.Add(New LineItem(LoadSQL("line_id"), LoadSQL("line_cd")))
			End While
		End If
		Return LineItems
	End Function
	Public Class LineItem
		Public Sub New(ByVal id As Integer, ByVal name As String)
			mID = id
			mName = name
		End Sub
		Private mID As Integer
		Public Property ID() As Integer
			Get
				Return mID
			End Get
			Set(ByVal value As Integer)
				mID = value
			End Set
		End Property

		Private mName As String
		Public Property Name() As String
			Get
				Return mName
			End Get
			Set(ByVal value As String)
				mName = value
			End Set
		End Property

	End Class
	Private Sub ComboBox3_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox3.SelectedIndexChanged
		'TextBox1.Text = ComboBox3.Items(ComboBox3.SelectedIndex)
		'ComboBox1.Items(ComboBox1.SelectedIndex) = TextBox1.Text
		Try
			ComboBox2.Items.Clear()
			Load_Line()
		Catch ex As Exception
			MsgBox(ex.Message)
		End Try

	End Sub
	Private Sub Panel1_Paint(sender As Object, e As PaintEventArgs) Handles Panel1.Paint

	End Sub
	Private Sub ComboBox7_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox7.SelectedIndexChanged

	End Sub
	Private Sub Label22_Click(sender As Object, e As EventArgs) Handles Label22.Click

	End Sub
	Private Sub ComboBox1_SelectedIndexChanged_1(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged

	End Sub

End Class
