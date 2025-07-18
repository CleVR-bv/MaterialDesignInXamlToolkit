using System.ComponentModel;
using System.Globalization;
using System.Windows.Media;
using MaterialDesignThemes.UITests.WPF.TextBoxes;


namespace MaterialDesignThemes.UITests.WPF.TimePickers;

public class TimePickerTests : TestBase
{
    [Test]
    [Arguments(1, 1, 1)]
    [Arguments(2020, 8, 10)]
    public async Task OnTextChangedIfSelectedTimeIsNonNull_DatePartDoesNotChange(int year, int month, int day)
    {
        await using var recorder = new TestRecorder(App);

        var stackPanel = await LoadXaml<StackPanel>(@"
<StackPanel>
    <materialDesign:TimePicker Language=""en-US"" />
</StackPanel>");
        var timePicker = await stackPanel.GetElement<TimePicker>("/TimePicker");
        var timePickerTextBox = await timePicker.GetElement<TimePickerTextBox>("/TimePickerTextBox");

        await timePicker.SetSelectedTime(new DateTime(year, month, day));
        await timePickerTextBox.MoveKeyboardFocus();
        await timePickerTextBox.SetText("1:10 AM");

        var actual = await timePicker.GetSelectedTime();
        await Assert.That(actual).IsEqualTo(new DateTime(year, month, day, 1, 10, 0));

        recorder.Success();
    }

    [Test]
    [Arguments(1, 1, 1)]
    [Arguments(2020, 8, 10)]
    public async Task OnLostFocusIfSelectedTimeIsNonNull_DatePartDoesNotChange(int year, int month, int day)
    {
        await using var recorder = new TestRecorder(App);

        var stackPanel = await LoadXaml<StackPanel>(@"
<StackPanel>
    <materialDesign:TimePicker Language=""en-US"" />
    <TextBox />
</StackPanel>");
        var timePicker = await stackPanel.GetElement<TimePicker>("/TimePicker");
        var timePickerTextBox = await timePicker.GetElement<TimePickerTextBox>("/TimePickerTextBox");
        var textBox = await stackPanel.GetElement<TextBox>("/TextBox");

        await timePicker.SetSelectedTime(new DateTime(year, month, day));
        await timePickerTextBox.MoveKeyboardFocus();
        await timePickerTextBox.SetText("1:10");
        await textBox.MoveKeyboardFocus();

        var actual = await timePicker.GetSelectedTime();
        await Assert.That(actual).IsEqualTo(new DateTime(year, month, day, 1, 10, 0));

        recorder.Success();
    }

    [Test]
    [Arguments(1, 1, 1)]
    [Arguments(2020, 8, 2)]
    public async Task OnClockPickedIfSelectedTimeIsNonNull_DatePartDoesNotChange(int year, int month, int day)
    {
        await using var recorder = new TestRecorder(App);

        var stackPanel = await LoadXaml<StackPanel>(@"
<StackPanel>
    <materialDesign:TimePicker/>
</StackPanel>");
        var timePicker = await stackPanel.GetElement<TimePicker>("/TimePicker");

        await timePicker.SetSelectedTime(new DateTime(year, month, day));
        await timePicker.PickClock(1, 10);

        var actual = await timePicker.GetSelectedTime();
        await Assert.That(actual).IsEqualTo(new DateTime(year, month, day, 1, 10, 0));

        recorder.Success();
    }

    [Test]
    public async Task OnTextChangedIfSelectedTimeIsNull_DatePartWillBeToday()
    {
        await using var recorder = new TestRecorder(App);

        var stackPanel = await LoadXaml<StackPanel>(@"
<StackPanel>
    <materialDesign:TimePicker Language=""en-US"" />
</StackPanel>");
        var timePicker = await stackPanel.GetElement<TimePicker>("/TimePicker");
        var timePickerTextBox = await timePicker.GetElement<TimePickerTextBox>("/TimePickerTextBox");

        var today = DateTime.Today;
        await timePickerTextBox.MoveKeyboardFocus();
        await timePickerTextBox.SetText("1:23 AM");

        var actual = await timePicker.GetSelectedTime();
        await Assert.That(actual).IsEqualTo(AdjustToday(today, actual).Add(new TimeSpan(1, 23, 0)));

        recorder.Success();
    }

    [Test]
    public async Task OnLostFocusIfSelectedTimeIsNull_DatePartWillBeToday()
    {
        await using var recorder = new TestRecorder(App);

        var stackPanel = await LoadXaml<StackPanel>(@"
<StackPanel>
    <materialDesign:TimePicker Language=""en-US"" />
    <TextBox />
</StackPanel>");
        var timePicker = await stackPanel.GetElement<TimePicker>("/TimePicker");
        var timePickerTextBox = await timePicker.GetElement<TimePickerTextBox>("/TimePickerTextBox");
        var textBox = await stackPanel.GetElement<TextBox>("/TextBox");

        var today = DateTime.Today;
        await timePickerTextBox.MoveKeyboardFocus();
        await timePickerTextBox.SetText("1:23");
        await textBox.MoveKeyboardFocus();

        var actual = await timePicker.GetSelectedTime();
        await Assert.That(actual).IsEqualTo(AdjustToday(today, actual).Add(new TimeSpan(1, 23, 0)));

        recorder.Success();
    }

    [Test]
    public async Task OnClockPickedIfSelectedTimeIsNull_DatePartWillBeToday()
    {
        await using var recorder = new TestRecorder(App);

        var stackPanel = await LoadXaml<StackPanel>(@"
<StackPanel>
    <materialDesign:TimePicker/>
</StackPanel>");
        var timePicker = await stackPanel.GetElement<TimePicker>("/TimePicker");

        var today = DateTime.Today;
        await timePicker.PickClock(1, 23);

        var actual = await timePicker.GetSelectedTime();
        await Assert.That(actual).IsEqualTo(AdjustToday(today, actual).Add(new TimeSpan(1, 23, 0)));

        recorder.Success();
    }

    private static DateTime AdjustToday(DateTime today, DateTime? adjustTo)
    {
        if (adjustTo.HasValue && today != adjustTo.Value.Date)
        {
            var tomorrow = today.AddDays(1);
            if (tomorrow == adjustTo.Value.Date)
                today = tomorrow;
        }
        return today;
    }

    [Test]
    [Arguments("1:2")]
    [Arguments("1:02")]
    [Arguments("1:02 AM")]
    public async Task OnLostFocusIfTimeHasBeenChanged_TextWillBeFormatted(string text)
    {
        await using var recorder = new TestRecorder(App);

        var stackPanel = await LoadXaml<StackPanel>(@"
<StackPanel>
    <materialDesign:TimePicker/>
    <TextBox/>
</StackPanel>");
        var timePicker = await stackPanel.GetElement<TimePicker>("/TimePicker");
        var timePickerTextBox = await timePicker.GetElement<TimePickerTextBox>("/TimePickerTextBox");
        var textBox = await stackPanel.GetElement<TextBox>("/TextBox");

        await timePickerTextBox.MoveKeyboardFocus();
        await timePickerTextBox.SendKeyboardInput($"{text}");
        await textBox.MoveKeyboardFocus();

        var actual = await timePickerTextBox.GetText();
        await Assert.That(actual).IsEqualTo("1:02 AM");

        recorder.Success();
    }

    [Test]
    [Arguments("1:2")]
    [Arguments("1:02")]
    [Arguments("1:02 AM")]
    public async Task OnLostFocusIfTimeHasNotBeenChanged_TextWillBeFormatted(string text)
    {
        await using var recorder = new TestRecorder(App);

        var stackPanel = await LoadXaml<StackPanel>(@"
<StackPanel>
    <materialDesign:TimePicker/>
    <TextBox/>
</StackPanel>");
        var timePicker = await stackPanel.GetElement<TimePicker>("/TimePicker");
        var timePickerTextBox = await timePicker.GetElement<TimePickerTextBox>("/TimePickerTextBox");
        var textBox = await stackPanel.GetElement<TextBox>("/TextBox");

        await timePicker.SetSelectedTime(new DateTime(2020, 8, 10, 1, 2, 0));
        await timePickerTextBox.MoveKeyboardFocus();
        await timePickerTextBox.SendKeyboardInput($"{text}");
        await textBox.MoveKeyboardFocus();

        var actual = await timePickerTextBox.GetText();
        await Assert.That(actual).IsEqualTo("1:02 AM");

        recorder.Success();
    }

    [Test]
    [Arguments("1:2")]
    [Arguments("1:02")]
    [Arguments("1:02 AM")]
    public async Task OnEnterKeyDownIfTimeHasNotBeenChanged_TextWillBeFormatted(string text)
    {
        await using var recorder = new TestRecorder(App);

        var stackPanel = await LoadXaml<StackPanel>(@"
<StackPanel>
    <materialDesign:TimePicker/>
</StackPanel>");
        var timePicker = await stackPanel.GetElement<TimePicker>("/TimePicker");
        var timePickerTextBox = await timePicker.GetElement<TimePickerTextBox>("/TimePickerTextBox");

        await timePicker.SetSelectedTime(new DateTime(2020, 8, 10, 1, 2, 0));
        await timePickerTextBox.MoveKeyboardFocus();
        await timePickerTextBox.SendKeyboardInput($"{text}{Key.Enter}");

        var actual = await timePickerTextBox.GetText();
        await Assert.That(actual).IsEqualTo("1:02 AM");

        recorder.Success();
    }

    [Test]
    [Arguments("1:2")]
    [Arguments("1:02")]
    [Arguments("1:02 AM")]
    public async Task OnEnterKeyDownIfTimeHasBeenChanged_TextWillBeFormatted(string text)
    {
        await using var recorder = new TestRecorder(App);

        var stackPanel = await LoadXaml<StackPanel>(@"
<StackPanel>
    <materialDesign:TimePicker/>
</StackPanel>");
        var timePicker = await stackPanel.GetElement<TimePicker>("/TimePicker");
        var timePickerTextBox = await timePicker.GetElement<TimePickerTextBox>("/TimePickerTextBox");

        await timePicker.SetSelectedTime(new DateTime(2020, 8, 10, 1, 3, 0));
        await timePickerTextBox.MoveKeyboardFocus();
        await timePickerTextBox.SendKeyboardInput($"{text}{Key.Enter}");

        var actual = await timePickerTextBox.GetText();
        await Assert.That(actual).IsEqualTo("1:02 AM");

        recorder.Success();
    }

    [Test]
    [Arguments("1:2")]
    [Arguments("1:02")]
    [Arguments("1:02 AM")]
    public async Task OnTimePickedIfTimeHasBeenChanged_TextWillBeFormatted(string text)
    {
        await using var recorder = new TestRecorder(App);

        var stackPanel = await LoadXaml<StackPanel>(@"
<StackPanel>
    <materialDesign:TimePicker/>
</StackPanel>");
        var timePicker = await stackPanel.GetElement<TimePicker>("/TimePicker");
        var timePickerTextBox = await timePicker.GetElement<TimePickerTextBox>("/TimePickerTextBox");

        await timePicker.SetSelectedTime(new DateTime(2020, 8, 10, 1, 2, 0));
        await timePickerTextBox.MoveKeyboardFocus();
        await timePickerTextBox.SendKeyboardInput($"{text}");
        await timePicker.PickClock(1, 3);

        var actual = await timePickerTextBox.GetText();
        await Assert.That(actual).IsEqualTo("1:03 AM");

        recorder.Success();
    }

    [Test]
    [Arguments("1:2")]
    [Arguments("1:02")]
    [Arguments("1:02 AM")]
    public async Task OnTimePickedIfTimeHasNotBeenChanged_TextWillBeFormatted(string text)
    {
        await using var recorder = new TestRecorder(App);

        var stackPanel = await LoadXaml<StackPanel>(@"
<StackPanel>
    <materialDesign:TimePicker/>
</StackPanel>");
        var timePicker = await stackPanel.GetElement<TimePicker>("/TimePicker");
        var timePickerTextBox = await timePicker.GetElement<TimePickerTextBox>("/TimePickerTextBox");

        await timePicker.SetSelectedTime(new DateTime(2020, 8, 10, 1, 2, 0));
        await timePickerTextBox.MoveKeyboardFocus();
        await timePickerTextBox.SendKeyboardInput($"{text}");
        await timePicker.PickClock(1, 2);

        var actual = await timePickerTextBox.GetText();
        await Assert.That(actual).IsEqualTo("1:02 AM");

        recorder.Success();
    }

    [Test]
    [Description("Pull Request 2192")]
    public async Task OnTimePickerHelperTextFontSize_ChangesHelperTextFontSize()
    {
        await using var recorder = new TestRecorder(App);

        var stackPanel = await LoadXaml<StackPanel>(@"
<StackPanel>
    <materialDesign:TimePicker
        materialDesign:HintAssist.HelperTextFontSize=""20""/>
</StackPanel>");
        var timePicker = await stackPanel.GetElement<TimePicker>("/TimePicker");
        var helpTextBlock = await timePicker.GetElement<TextBlock>("/Grid/Canvas/TextBlock");

        double fontSize = await helpTextBlock.GetFontSize();

        await Assert.That(fontSize).IsEqualTo(20);
        recorder.Success();
    }

    [Test]
    [Description("Issue 2737")]
    public async Task OutlinedTimePicker_RespectsActiveAndInactiveBorderThickness_WhenAttachedPropertiesAreSet()
    {
        await using var recorder = new TestRecorder(App);

        // Arrange
        var expectedInactiveBorderThickness = new Thickness(4, 3, 2, 1);
        var expectedActiveBorderThickness = new Thickness(1, 2, 3, 4);
        var stackPanel = await LoadXaml<StackPanel>($@"
<StackPanel>
    <materialDesign:TimePicker Style=""{{StaticResource MaterialDesignOutlinedTimePicker}}""
      materialDesign:TimePickerAssist.OutlinedBorderInactiveThickness=""{expectedInactiveBorderThickness}""
      materialDesign:TimePickerAssist.OutlinedBorderActiveThickness=""{expectedActiveBorderThickness}"">
      <materialDesign:TimePicker.Text>
        <Binding RelativeSource=""{{RelativeSource Self}}"" Path=""Tag"" UpdateSourceTrigger=""PropertyChanged"">
          <Binding.ValidationRules>
            <local:OnlyTenOClockValidationRule ValidatesOnTargetUpdated=""True""/>
          </Binding.ValidationRules>
        </Binding>
      </materialDesign:TimePicker.Text>
    </materialDesign:TimePicker>
    <Button x:Name=""Button"" Content=""Some Button"" Margin=""0,20,0,0"" />
</StackPanel>", ("local", typeof(OnlyTenOClockValidationRule)));
        var timePicker = await stackPanel.GetElement<TimePicker>("/TimePicker");
        await timePicker.SetProperty(TimePicker.TextProperty, "10:00");
        var timePickerTextBox = await timePicker.GetElement<TimePickerTextBox>("/TimePickerTextBox");
        var textBoxOuterBorder = await timePickerTextBox.GetElement<Border>("OuterBorder");
        var button = await stackPanel.GetElement<Button>("Button");

        // Act
        await button.MoveCursorTo();
        await Task.Delay(50, TestContext.Current!.CancellationToken);   // Wait for the visual change
        var inactiveBorderThickness = await textBoxOuterBorder.GetBorderThickness();
        await timePickerTextBox.MoveCursorTo();
        await Task.Delay(50, TestContext.Current.CancellationToken);   // Wait for the visual change
        var hoverBorderThickness = await textBoxOuterBorder.GetBorderThickness(); ;
        await timePickerTextBox.LeftClick();
        await Task.Delay(50, TestContext.Current.CancellationToken);   // Wait for the visual change
        var focusedBorderThickness = await textBoxOuterBorder.GetBorderThickness(); ;

        // TODO: It would be cool if a validation error could be set via XAMLTest without the need for the Binding and ValidationRules elements in the XAML above.
        await timePicker.SetProperty(TimePicker.TextProperty, "11:00");
        await Task.Delay(50, TestContext.Current.CancellationToken);   // Wait for the visual change
        var withErrorBorderThickness = await textBoxOuterBorder.GetBorderThickness(); ;

        // Assert
        await Assert.That(inactiveBorderThickness).IsEqualTo(expectedInactiveBorderThickness);
        await Assert.That(hoverBorderThickness).IsEqualTo(expectedActiveBorderThickness);
        await Assert.That(focusedBorderThickness).IsEqualTo(expectedActiveBorderThickness);
        await Assert.That(withErrorBorderThickness).IsEqualTo(expectedActiveBorderThickness);

        recorder.Success();
    }

    [Test]
    [Arguments("MaterialDesignFloatingHintTimePicker", null)]
    [Arguments("MaterialDesignFloatingHintTimePicker", 5)]
    [Arguments("MaterialDesignFloatingHintTimePicker", 20)]
    [Arguments("MaterialDesignFilledTimePicker", null)]
    [Arguments("MaterialDesignFilledTimePicker", 5)]
    [Arguments("MaterialDesignFilledTimePicker", 20)]
    [Arguments("MaterialDesignOutlinedTimePicker", null)]
    [Arguments("MaterialDesignOutlinedTimePicker", 5)]
    [Arguments("MaterialDesignOutlinedTimePicker", 20)]
    public async Task TimePicker_WithHintAndHelperText_RespectsPadding(string styleName, int? padding)
    {
        await using var recorder = new TestRecorder(App);

        // FIXME: Tolerance needed because TextFieldAssist.TextBoxViewMargin is in play and slightly modifies the hint text placement in certain cases.
        const double tolerance = 1.5;

        string styleAttribute = $"Style=\"{{StaticResource {styleName}}}\"";
        string paddingAttribute = padding.HasValue ? $"Padding=\"{padding.Value}\"" : string.Empty;

        var timePicker = await LoadXaml<TimePicker>($@"
<materialDesign:TimePicker {styleAttribute} {paddingAttribute}
  materialDesign:HintAssist.Hint=""Hint text""
  materialDesign:HintAssist.HelperText=""Helper text""
  Width=""200"" VerticalAlignment=""Center"" HorizontalAlignment=""Center"" />
");

        var contentHost = await timePicker.GetElement<ScrollViewer>("PART_ContentHost");
        var hint = await timePicker.GetElement<SmartHint>("Hint");
        var helperText = await timePicker.GetElement<TextBlock>("HelperTextTextBlock");

        Rect? contentHostCoordinates = await contentHost.GetCoordinates();
        Rect? hintCoordinates = await hint.GetCoordinates();
        Rect? helperTextCoordinates = await helperText.GetCoordinates();

        await Assert.That(Math.Abs(contentHostCoordinates.Value.Left - hintCoordinates.Value.Left)).IsCloseTo(0, tolerance);
        await Assert.That(Math.Abs(contentHostCoordinates.Value.Left - helperTextCoordinates.Value.Left)).IsCloseTo(0, tolerance);

        recorder.Success();
    }

    [Test]
    [Arguments("MaterialDesignFloatingHintTimePicker", null)]
    [Arguments("MaterialDesignFloatingHintTimePicker", 5)]
    [Arguments("MaterialDesignFloatingHintTimePicker", 20)]
    [Arguments("MaterialDesignFilledTimePicker", null)]
    [Arguments("MaterialDesignFilledTimePicker", 5)]
    [Arguments("MaterialDesignFilledTimePicker", 20)]
    [Arguments("MaterialDesignOutlinedTimePicker", null)]
    [Arguments("MaterialDesignOutlinedTimePicker", 5)]
    [Arguments("MaterialDesignOutlinedTimePicker", 20)]
    public async Task TimePicker_WithHintAndValidationError_RespectsPadding(string styleName, int? padding)
    {
        await using var recorder = new TestRecorder(App);

        // FIXME: Tolerance needed because TextFieldAssist.TextBoxViewMargin is in play and slightly modifies the hint text placement in certain cases.
        const double tolerance = 1.5;

        string styleAttribute = $"Style=\"{{StaticResource {styleName}}}\"";
        string paddingAttribute = padding.HasValue ? $"Padding=\"{padding.Value}\"" : string.Empty;

        var timePicker = await LoadXaml<TimePicker>($@"
<materialDesign:TimePicker {styleAttribute} {paddingAttribute}
  materialDesign:HintAssist.Hint=""Hint text""
  materialDesign:HintAssist.HelperText=""Helper text""
  Width=""200"" VerticalAlignment=""Center"" HorizontalAlignment=""Center"">
  <materialDesign:TimePicker.Text>
    <Binding RelativeSource=""{{RelativeSource Self}}"" Path=""Tag"" UpdateSourceTrigger=""PropertyChanged"">
      <Binding.ValidationRules>
        <local:NotEmptyValidationRule ValidatesOnTargetUpdated=""True""/>
      </Binding.ValidationRules>
    </Binding>
  </materialDesign:TimePicker.Text>
</materialDesign:TimePicker>
", ("local", typeof(NotEmptyValidationRule)));

        var contentHost = await timePicker.GetElement<ScrollViewer>("PART_ContentHost");
        var hint = await timePicker.GetElement<SmartHint>("Hint");
        var errorViewer = await timePicker.GetElement<Border>("DefaultErrorViewer");

        Rect? contentHostCoordinates = await contentHost.GetCoordinates();
        Rect? hintCoordinates = await hint.GetCoordinates();
        Rect? errorViewerCoordinates = await errorViewer.GetCoordinates();

        await Assert.That(Math.Abs(contentHostCoordinates.Value.Left - hintCoordinates.Value.Left)).IsCloseTo(0, tolerance);
        await Assert.That(Math.Abs(contentHostCoordinates.Value.Left - errorViewerCoordinates.Value.Left)).IsCloseTo(0, tolerance);

        recorder.Success();
    }

    [Test]
    [Description("Issue 3119")]
    public async Task TimePicker_WithClearButton_ClearButtonClearsSelectedTime()
    {
        await using var recorder = new TestRecorder(App);

        // Arrange
        var stackPanel = await LoadXaml<StackPanel>($@"
<StackPanel>
  <materialDesign:TimePicker
    SelectedTime=""08:30""
    materialDesign:TextFieldAssist.HasClearButton=""True"" />
</StackPanel>");
        var timePicker = await stackPanel.GetElement<TimePicker>("/TimePicker");
        var clearButton = await timePicker.GetElement<Button>("PART_ClearButton");
        await Assert.That(await timePicker.GetSelectedTime()).IsNotNull();

        // Act
        await clearButton.LeftClick();
        await Task.Delay(50, TestContext.Current!.CancellationToken);

        // Assert
        await Assert.That(await timePicker.GetSelectedTime()).IsNull();

        recorder.Success();
    }

    [Test]
    [Description("Issue 3369")]
    public async Task TimePicker_WithClearButton_ClearButtonClearsUncommittedText()
    {
        await using var recorder = new TestRecorder(App);

        // Arrange
        var stackPanel = await LoadXaml<StackPanel>($"""
        <StackPanel>
          <materialDesign:TimePicker
             materialDesign:TextFieldAssist.HasClearButton="True" />
        </StackPanel>
        """);
        var timePicker = await stackPanel.GetElement<TimePicker>("/TimePicker");
        var timePickerTextBox = await timePicker.GetElement<TimePickerTextBox>("/TimePickerTextBox");
        var clearButton = await timePicker.GetElement<Button>("PART_ClearButton");

        await timePickerTextBox.SendKeyboardInput($"invalid time");
        await Assert.That(await timePickerTextBox.GetText()).IsEqualTo("invalid time");

        // Act
        await clearButton.LeftClick();
        await Task.Delay(50, TestContext.Current!.CancellationToken);

        // Assert
        await Assert.That(await timePickerTextBox.GetText()).IsNull();

        recorder.Success();
    }

    [Test]
    [Description("Issue 3365")]
    public async Task TimePicker_WithOutlinedStyleAndNoCustomHintBackgroundSet_ShouldApplyDefaultBackgroundWhenFloated()
    {
        await using var recorder = new TestRecorder(App);

        // Arrange
        var stackPanel = await LoadXaml<StackPanel>("""
            <StackPanel>
            <materialDesign:TimePicker
              Style="{StaticResource MaterialDesignOutlinedTimePicker}"
              materialDesign:HintAssist.Hint="Hint text" />
            </StackPanel>
            """);
        var timePicker = await stackPanel.GetElement<TimePicker>("/TimePicker");
        var timePickerTextBox = await timePicker.GetElement<TimePickerTextBox>("/TimePickerTextBox");
        var hintBackgroundGrid = await timePicker.GetElement<Grid>("HintBackgroundGrid");

        var defaultFloatedBackground = await GetThemeColor("MaterialDesign.Brush.Background");

        // Assert (unfocused state)
        await Assert.That(await hintBackgroundGrid.GetBackgroundColor()).IsNull();

        // Act
        await timePickerTextBox.MoveKeyboardFocus();

        // Assert (focused state)
        await Assert.That(await hintBackgroundGrid.GetBackgroundColor()).IsEqualTo(defaultFloatedBackground);

        recorder.Success();
    }

    [Test]
    [Description("Issue 3365")]
    [Arguments("MaterialDesignTimePicker")]
    [Arguments("MaterialDesignFloatingHintTimePicker")]
    [Arguments("MaterialDesignFilledTimePicker")]
    [Arguments("MaterialDesignOutlinedTimePicker")]
    public async Task TimePicker_WithCustomHintBackgroundSet_ShouldApplyHintBackground(string style)
    {
        await using var recorder = new TestRecorder(App);

        // Arrange
        var stackPanel = await LoadXaml<StackPanel>($$"""
            <StackPanel>
              <materialDesign:TimePicker
                Style="{StaticResource {{style}}}"
                materialDesign:HintAssist.Hint="Hint text"
                materialDesign:HintAssist.Background="Red" />
            </StackPanel>
            """);
        var timePicker = await stackPanel.GetElement<TimePicker>("/TimePicker");
        var timePickerTextBox = await timePicker.GetElement<TimePickerTextBox>("/TimePickerTextBox");
        var hintBackgroundBorder = await timePicker.GetElement<Border>("HintBackgroundBorder");

        // Assert (unfocused state)
        await Assert.That(await hintBackgroundBorder.GetBackgroundColor()).IsEqualTo(Colors.Red);

        // Act
        await timePickerTextBox.MoveKeyboardFocus();

        // Assert (focused state)
        await Assert.That(await hintBackgroundBorder.GetBackgroundColor()).IsEqualTo(Colors.Red);

        recorder.Success();
    }

    [Test]
    [Description("Issue 3547")]
    public async Task TimePicker_ShouldApplyIsMouseOverTriggers_WhenHoveringTimeButton()
    {
        await using var recorder = new TestRecorder(App);

        // Arrange
        Thickness expectedThickness = Constants.DefaultOutlinedBorderActiveThickness;
        var stackPanel = await LoadXaml<StackPanel>("""
            <StackPanel>
              <materialDesign:TimePicker
                Style="{StaticResource MaterialDesignOutlinedTimePicker}" />
            </StackPanel>
            """);
        var timePicker = await stackPanel.GetElement<TimePicker>("/TimePicker");
        var timePickerTextBox = await timePicker.GetElement<TimePickerTextBox>("/TimePickerTextBox");
        var timePickerTextBoxBorder = await timePickerTextBox.GetElement<Border>("OuterBorder");
        var timePickerTimeButton = await timePicker.GetElement<Button>("PART_Button");

        // Act
        await timePickerTextBoxBorder.MoveCursorTo();
        await Task.Delay(50, TestContext.Current!.CancellationToken);
        var timePickerTextBoxHoverThickness = await timePickerTextBoxBorder.GetBorderThickness();
        await timePickerTimeButton.MoveCursorTo();
        await Task.Delay(50, TestContext.Current.CancellationToken);
        var timePickerTimeButtonHoverThickness = await timePickerTextBoxBorder.GetBorderThickness();

        // Assert
        await Assert.That(timePickerTextBoxHoverThickness).IsEqualTo(expectedThickness);
        await Assert.That(timePickerTimeButtonHoverThickness).IsEqualTo(expectedThickness);

        recorder.Success();
    }

    [Test]
    [Description("Issue 3650")]
    public async Task TimePicker_MovesFocusToPrevious_WhenShiftAndTabIsPressed()
    {
        await using var recorder = new TestRecorder(App);

        // Arrange
        var stackPanel = await LoadXaml<StackPanel>("""
            <StackPanel>
              <TextBox />
              <materialDesign:TimePicker />
            </StackPanel>
            """);

        var textBox = await stackPanel.GetElement<TextBox>("/TextBox");
        var timePickerTextBox = await stackPanel.GetElement<TimePickerTextBox>("/TimePickerTextBox");

        // Act
        await timePickerTextBox.MoveKeyboardFocus();
        await Task.Delay(50, TestContext.Current!.CancellationToken);
        await timePickerTextBox.SendInput(new KeyboardInput(Key.LeftShift, Key.Tab));
        await Task.Delay(50, TestContext.Current.CancellationToken);

        // Assert
        await Assert.That(await textBox.GetIsFocused()).IsTrue();
        await Assert.That(await timePickerTextBox.GetIsFocused()).IsFalse();

        recorder.Success();
    }
}

public class OnlyTenOClockValidationRule : ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        => value is not "10:00" ? new ValidationResult(false, "Only 10 o'clock allowed") : ValidationResult.ValidResult;
}
