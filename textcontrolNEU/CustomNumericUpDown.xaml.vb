Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Data
Imports System.Windows.Documents
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports System.Windows.Navigation
Imports System.Windows.Shapes
Imports System.Timers
Imports System.ComponentModel

''' <summary>
''' Interaction logic for CustomNumericUpDown.xaml
''' </summary>
Partial Public Class CustomNumericUpDown
	Inherits UserControl

	Public Shared ReadOnly ValueProperty As DependencyProperty

	Private Delegate Sub SetValueCallback(value As Decimal)

	Private _setValCallb As SetValueCallback
	Private _value As Decimal
	Private _lastValue As Decimal
	Private _minimum As Decimal
	Private _maximum As Decimal
	Private _timer As Timer
	Private _delta As Double
	Private _deltaInc As Double

	Private Const MinDefault As Decimal = 1
	Private Const MaxDefault As Decimal = 100

	Shared Sub New()
		ValueProperty = DependencyProperty.Register("Value", GetType(Decimal), GetType(CustomNumericUpDown), New FrameworkPropertyMetadata(MinDefault, New PropertyChangedCallback(AddressOf OnValueChanged)))
	End Sub

	Public Sub New()
		InitializeComponent()

		_setValCallb = New SetValueCallback(AddressOf CheckAndSetValue)

		_timer = New Timer(200)
		_timer.AutoReset = True
		AddHandler _timer.Elapsed, AddressOf TimerElapsed

		_minimum = MinDefault
		_maximum = MaxDefault

		_value = _lastValue = _minimum
		_textBox.Text = _value.ToString()
	End Sub

	Public Property TextAlignment() As TextAlignment
		Get
			Return _textBox.TextAlignment
		End Get
		Set(value As TextAlignment)
			_textBox.TextAlignment = value
		End Set
	End Property

	Public Property Minimum() As Decimal
		Get
			Return _minimum
		End Get

		Set(value As Decimal)
			_minimum = value
			_maximum = Math.Max(_minimum, _maximum)
			CheckValue()
		End Set
	End Property

	Public Property Maximum() As Decimal
		Get
			Return _maximum
		End Get

		Set(value As Decimal)
			_maximum = value
			_minimum = Math.Min(_minimum, _maximum)
			CheckValue()
		End Set
	End Property

	<Category("Value"), Bindable(True)> _
	Public Property Value() As Decimal
		Get
			Return CDec(GetValue(ValueProperty))
		End Get

		Set(value As Decimal)
			_bReenableTimer = False
			_timer.[Stop]()
			CheckAndSetValue(value)
		End Set
	End Property

	''' <summary>
	''' Create a custom routed event by first registering a RoutedEventID 
	''' This event uses the bubbling routing strategy 
	''' </summary>
	Public Shared ReadOnly ValueChangedEvent As RoutedEvent = EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Bubble, GetType(RoutedEventHandler), GetType(CustomNumericUpDown))

	''' <summary>
	''' Is fired when the value is changed.
	''' </summary>
	<Category("Behaviour")> _
	<Description("Occurs when the value is changed.")> _
	Public Custom Event ValueChanged As RoutedEventHandler
		RaiseEvent(sender As Object, e As RoutedEventArgs)
			[RaiseEvent](e)
		End RaiseEvent
		AddHandler(ByVal value As RoutedEventHandler)
			[AddHandler](ValueChangedEvent, value)
		End AddHandler
		RemoveHandler(ByVal value As RoutedEventHandler)
			[RemoveHandler](ValueChangedEvent, value)
		End RemoveHandler
	End Event

	''' <summary>
	''' This method raises the ValueChanged event
	''' </summary>
	Private Sub RaiseValueChangedEvent()
		Dim args = New RoutedEventArgs(CustomNumericUpDown.ValueChangedEvent)
		[RaiseEvent](args)
	End Sub

	Private Shared Sub OnValueChanged(obj As DependencyObject, args As DependencyPropertyChangedEventArgs)
		Dim custNumUpDn = TryCast(obj, CustomNumericUpDown)
		If custNumUpDn Is Nothing Then
			Return
		End If

		custNumUpDn.RaiseValueChangedEvent()
	End Sub

	Private _bReenableTimer As Boolean
	Private Sub CheckAndSetValue(value As Decimal)
		_bReenableTimer = _timer.Enabled
		If _bReenableTimer Then
			_timer.[Stop]()
		End If

		_value = value
		CheckValue()

		RemoveHandler _textBox.TextChanged, AddressOf TextBoxValue_TextChanged
		_textBox.Text = _value.ToString()
		AddHandler _textBox.TextChanged, AddressOf TextBoxValue_TextChanged

		' Set dependency property
		SetValue(ValueProperty, _value)

		If _bReenableTimer Then
			_timer.Start()
		End If
	End Sub

	Private Sub CheckValue()
		_value = Math.Max(_minimum, Math.Min(_maximum, _value))
	End Sub

	Private Sub TimerElapsed(sender As Object, e As ElapsedEventArgs)
		Me.Dispatcher.BeginInvoke(_setValCallb, New Object() {CDec(CInt(Math.Truncate(_value + CDec(_delta))))})
		_delta += _deltaInc
	End Sub

	Private Sub ButtonUp_PreviewMouseLeftButtonDown(sender As Object, e As MouseButtonEventArgs)
		_delta = 1
		_deltaInc = 0.2
		_timer.Start()

		Me.Dispatcher.BeginInvoke(_setValCallb, New Object() {_value + 1})
	End Sub

	Private Sub ButtonUp_PreviewMouseLeftButtonUp(sender As Object, e As MouseButtonEventArgs)
		_timer.[Stop]()
	End Sub

	Private Sub ButtonDown_PreviewMouseLeftButtonDown(sender As Object, e As MouseButtonEventArgs)
		_delta = -1
		_deltaInc = -0.2
		_timer.Start()

		Me.Dispatcher.BeginInvoke(_setValCallb, New Object() {_value - 1})
	End Sub

	Private Sub ButtonDown_PreviewMouseLeftButtonUp(sender As Object, e As MouseButtonEventArgs)
		_timer.[Stop]()
	End Sub

	Private Sub TextBoxValue_GotFocus(sender As Object, e As RoutedEventArgs)
		_lastValue = _value
	End Sub

	Private Sub TextBoxValue_LostFocus(sender As Object, e As RoutedEventArgs)
		Try
			CheckAndSetValue(Convert.ToInt32(_textBox.Text))
		Catch
			_textBox.Text = _lastValue.ToString()
			_value = _lastValue
		End Try
	End Sub

	Private Sub TextBoxValue_TextChanged(sender As Object, e As TextChangedEventArgs)
		Try
			CheckAndSetValue(Convert.ToInt32(_textBox.Text))
		Catch
			_value = _lastValue
			SetValue(ValueProperty, _value)
		End Try
	End Sub
End Class
