using Library.Core;
using Library.Desktop;
using Library.Desktop.Services;
using System.Diagnostics;

internal static class Program
{
    [STAThread]
    private static int Main(string[] args)
    {
        ApplicationConfiguration.Initialize();
        // Test failures must reach the console instead of opening an invisible WinForms error dialog.
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);
        var output = Path.GetFullPath(args.Length > 0 ? args[0] : "artifacts/screenshots"); Directory.CreateDirectory(output);
        try
        {
            // No runtime dependencies: the same parameterless constructors are used by VS Designer.
            using (var design = new MainForm())
            {
                design.ShowInTaskbar = false; design.Opacity = 0; design.Show(); Application.DoEvents();
                var tabs = Find<TabControl>(design, "tabs");
                foreach (TabPage page in tabs.TabPages)
                {
                    tabs.SelectedTab = page; Application.DoEvents();
                    using var bitmap = new Bitmap(design.Width, design.Height); design.DrawToBitmap(bitmap, new Rectangle(0, 0, design.Width, design.Height));
                    bitmap.Save(Path.Combine(output, page.Name + ".png"));
                }
                design.Close();
            }
            Console.WriteLine("PASS: All WinForms Designer surfaces instantiate without a server.");
            using (var loginDesign = new LoginForm())
            {
                loginDesign.ShowInTaskbar = false; loginDesign.Opacity = 0; loginDesign.Show(); Application.DoEvents();
                var user = Find<TextBox>(loginDesign, "userBox");
                var password = Find<TextBox>(loginDesign, "passwordBox");
                var url = Find<TextBox>(loginDesign, "urlBox");
                var button = Find<Button>(loginDesign, "loginButton");
                if (button.Enabled) throw new Exception("Login must be disabled while required fields are empty.");
                user.Text = "librarian";
                if (button.Enabled) throw new Exception("Username alone must not enable login.");
                password.Text = "Library@123";
                if (!button.Enabled) throw new Exception("TextChanged events did not enable login for valid input.");
                url.Text = "not-a-server";
                if (button.Enabled) throw new Exception("An invalid server URL must disable login.");
                url.Text = "http://localhost:5080";
                if (!button.Enabled) throw new Exception("Correcting the server URL must enable login.");
                password.Clear();
                if (button.Enabled) throw new Exception("Clearing the password must disable login.");
                user.Clear();
                using var screenshot = new Bitmap(loginDesign.Width, loginDesign.Height);
                loginDesign.DrawToBitmap(screenshot, new Rectangle(0, 0, loginDesign.Width, loginDesign.Height));
                screenshot.Save(Path.Combine(output, "loginForm.png"));
                loginDesign.Close();
            }
            Console.WriteLine("PASS: LoginForm Load/TextChanged events validate required fields and server URL without a server.");
            VerifyEditorEvents();
            if (args.Contains("--offline")) return 0;
            using var api = new LibraryApiClient();
            using (var login = new LoginForm(api))
            {
                login.ShowInTaskbar = false; login.Opacity = 0; login.Show();
                Find<TextBox>(login, "userBox").Text = "missing_smoke_user"; Find<TextBox>(login, "passwordBox").Text = "Wrong@123";
                Find<Button>(login, "loginButton").PerformClick(); PumpUntil(() => Find<Button>(login, "loginButton").Enabled, "Rejected desktop login");
                if (api.CurrentUser != null) throw new Exception("Invalid login was accepted.");
                if (!Find<Label>(login, "statusLabel").Text.Contains("không hợp lệ")) throw new Exception("Invalid login did not display the authentication error.");
                Find<TextBox>(login, "userBox").Text = "librarian"; Find<TextBox>(login, "passwordBox").Text = "Library@123";
                Find<Button>(login, "loginButton").PerformClick(); PumpUntil(() => api.CurrentUser != null || Find<Button>(login, "loginButton").Enabled, "Desktop login");
                if (api.CurrentUser == null) throw new Exception(Find<Label>(login, "statusLabel").Text);
            }
            using var readerApi = new LibraryApiClient(); readerApi.SetBaseUrl("http://localhost:5080"); Pump(readerApi.LoginAsync("manager", "Library@123"));
            var suffix = Guid.NewGuid().ToString("N")[..8]; var username = "SMOKE_" + suffix;
            Pump(readerApi.SaveAsync("api/users", new UserRequest(username, "Bạn đọc smoke test", "Library@123", Roles.Reader, true)));
            var readers = Pump(readerApi.GetAsync<Page<UserDto>>("api/users?q=" + username)); var reader = readers.Items.Single();
            var book = Pump(api.GetAsync<Page<BookDto>>("api/books")).Items.First(); var code = "SMOKE_" + suffix.ToUpperInvariant();
            Pump(api.SaveAsync("api/copies", new CopyRequest(book.Id, code, "Kệ thử nghiệm")));
            using var main = new MainForm(api); main.ShowInTaskbar = false; main.Opacity = 0; main.Show();
            Find<TextBox>(main, "readerSearch").Text = username;
            ClickAndWait(main, "findButton"); Find<TextBox>(main, "codesBox").Text = code;
            ClickAndWait(main, "borrowButton");
            var loans = Pump(api.GetAsync<Page<LoanDto>>($"api/loans?readerId={reader.Id}&active=true")); if (loans.Total != 1) throw new Exception("Desktop borrow did not commit.");
            var afterBorrow = Pump(api.GetAsync<BookDto>($"api/books/{book.Id}")); if (afterBorrow.Available != book.Available) throw new Exception("Availability did not decrease after borrow.");
            Find<TextBox>(main, "returnBox").Text = code; ClickAndWait(main, "returnButton");
            if (Pump(api.GetAsync<Page<LoanDto>>($"api/loans?readerId={reader.Id}&active=true")).Total != 0) throw new Exception("Desktop return did not commit.");
            var history = Pump(api.GetAsync<Page<LoanDto>>($"api/loans?readerId={reader.Id}")); if (history.Items.Single().Status != "RETURNED") throw new Exception("History is inconsistent.");
            // Keep the seed demo clean: the smoke assets remain auditable but are not offered for circulation.
            var copy = Pump(api.GetAsync<Page<CopyDto>>("api/copies?q=" + code)).Items.Single();
            Pump(api.SaveAsync($"api/copies/{copy.Id}", new CopyUpdate("Kệ thử nghiệm", "INACTIVE"), true));
            Pump(readerApi.SaveAsync($"api/users/{reader.Id}", new UserUpdate(reader.FullName, Roles.Reader, false), true));
            Console.WriteLine("PASS: WinForms login → find reader → borrow → shared catalog → return → history (real HTTP + SQL Server).");

            // Exercise real Load/Click wiring after moving connection setup to the hosted controls' Load events.
            var liveTabs = Find<TabControl>(main, "tabs");
            Check(!liveTabs.TabPages.ContainsKey("policyTab"), "Librarian must not see the policy tab.");
            foreach (var (tabName, controlName, buttonName) in new[]
            {
                ("catalogTab", "catalogControl", "loadButton"),
                ("copiesTab", "copiesControl", "searchButton"),
                ("accountsTab", "accountsControl", "searchButton"),
                ("historyTab", "historyControl", "searchButton"),
                ("metadataTab", "metadataControl", "loadButton")
            })
            {
                Console.WriteLine("CHECK: Loading " + controlName);
                liveTabs.SelectedTab = liveTabs.TabPages[tabName];
                Application.DoEvents();
                var editor = Find<UserControl>(main, controlName);
                ClickAndWait(editor, buttonName);
                Check(Find<DataGridView>(editor, "grid").DataSource != null, controlName + " did not load its data.");
                Console.WriteLine("PASS: Loaded " + controlName);
            }
            using (var managerForm = new MainForm(readerApi))
            {
                Console.WriteLine("CHECK: Loading Manager policy screen");
                managerForm.ShowInTaskbar = false;
                managerForm.Opacity = 0;
                managerForm.Show();
                var managerTabs = Find<TabControl>(managerForm, "tabs");
                managerTabs.SelectedTab = managerTabs.TabPages["policyTab"];
                Application.DoEvents();
                ClickAndWait(Find<UserControl>(managerForm, "policyControl"), "loadButton");
                managerForm.Close();
            }
            Console.WriteLine("PASS: All live management screens load through Designer-wired events; Manager/Librarian tabs respect their roles.");
            main.Close(); return 0;
        }
        catch (Exception ex) { Console.Error.WriteLine(ex); return 1; }
    }
    private static T Find<T>(Control parent, string name) where T : Control => parent.Controls.Find(name, true).OfType<T>().Single();
    private static void ClickAndWait(Control parent, string name)
    {
        var button = Find<Button>(parent, name);
        if (!button.Enabled) throw new Exception(name + " is disabled before the action.");
        button.PerformClick();
        PumpUntil(() => button.Parent!.Enabled, name);
        var status = Find<Label>(button.Parent!, "statusLabel").Text; if (!status.StartsWith("Hoàn tất")) throw new Exception(name + ": " + status);
    }
    private static void PumpUntil(Func<bool> done, string operation)
    {
        var timer = Stopwatch.StartNew(); do { Application.DoEvents(); Thread.Sleep(15); if (timer.Elapsed > TimeSpan.FromSeconds(30)) throw new TimeoutException(operation); } while (!done());
    }
    private static T Pump<T>(Task<T> task) { PumpUntil(() => task.IsCompleted, "HTTP request"); return task.GetAwaiter().GetResult(); }
    private static void Pump(Task task) { PumpUntil(() => task.IsCompleted, "HTTP request"); task.GetAwaiter().GetResult(); }

    private static void VerifyEditorEvents()
    {
        using var shell = new MainForm();
        shell.ShowInTaskbar = false;
        shell.Opacity = 0;
        shell.Show();
        var tabs = Find<TabControl>(shell, "tabs");

        tabs.SelectedTab = tabs.TabPages["circulationTab"];
        Application.DoEvents();
        var circulation = Find<Library.Desktop.Controls.CirculationControl>(shell, "circulationControl");
        var borrow = Find<Button>(circulation, "borrowButton");
        var returnButton = Find<Button>(circulation, "returnButton");
        Check(!borrow.Enabled && !returnButton.Enabled, "Empty circulation inputs must disable confirmation.");
        Find<TextBox>(circulation, "readerSearch").Text = "SV001";
        var readerCombo = Find<ComboBox>(circulation, "readerCombo");
        readerCombo.DisplayMember = "UserName";
        readerCombo.DataSource = new List<UserDto> { new("r1", "SV001", "Bạn đọc mẫu", Roles.Reader, true) };
        Find<TextBox>(circulation, "codesBox").Text = "CP001";
        Check(borrow.Enabled, "Selecting a reader and entering a copy must enable borrowing.");
        Find<TextBox>(circulation, "readerSearch").Text = "SV002";
        Check(!borrow.Enabled && readerCombo.SelectedItem == null, "Changing reader search must clear the old borrower.");
        Find<TextBox>(circulation, "returnBox").Text = "CP001";
        Check(returnButton.Enabled, "Entering a return code must enable return.");
        Find<TextBox>(circulation, "returnBox").Clear();
        Check(!returnButton.Enabled, "Clearing a return code must disable return.");

        tabs.SelectedTab = tabs.TabPages["catalogTab"];
        Application.DoEvents();
        var catalog = Find<Library.Desktop.Controls.CatalogControl>(shell, "catalogControl");
        var authors = Find<CheckedListBox>(catalog, "authorsList");
        authors.DataSource = new List<NamedDto> { new(1, "Tác giả mẫu") };
        authors.DisplayMember = "Name";
        foreach (var comboName in new[] { "categoryCombo", "publisherCombo" })
        {
            var combo = Find<ComboBox>(catalog, comboName);
            combo.DisplayMember = "Name";
            combo.ValueMember = "Id";
            combo.DataSource = new List<NamedDto> { new(1, "Danh mục mẫu") };
        }
        Find<DataGridView>(catalog, "grid").DataSource = new List<BookDto>
        {
            new(1, "Sách mẫu", "9786040000000", 2026, "Mô tả", "images/books/default.svg", 1, "Thể loại", 1, "NXB", [1], "Tác giả mẫu", true, 2, 2)
        };
        Application.DoEvents();
        Check(Find<TextBox>(catalog, "titleBox").Text == "Sách mẫu" && authors.CheckedItems.Count == 1, "Selecting a book must fill the editor and author checkboxes.");
        Find<TextBox>(catalog, "imageBox").Text = "D:/books/a.png";
        Check(Find<Label>(catalog, "statusLabel").Text.Contains("đường dẫn tương đối"), "Image editing must show relative-path feedback.");
        Find<Button>(catalog, "newButton").PerformClick();
        Check(Find<TextBox>(catalog, "titleBox").TextLength == 0 && authors.CheckedItems.Count == 0, "New book must clear the previous selection.");

        tabs.SelectedTab = tabs.TabPages["copiesTab"];
        Application.DoEvents();
        var copies = Find<Library.Desktop.Controls.CopiesControl>(shell, "copiesControl");
        Find<DataGridView>(copies, "grid").DataSource = new List<CopyDto> { new(1, 1, "CP001", "Kệ A", "ON_LOAN") };
        Application.DoEvents();
        Check(Find<TextBox>(copies, "codeBox").ReadOnly && !Find<ComboBox>(copies, "stateCombo").Enabled, "An on-loan copy must protect its immutable code and circulation state.");
        Find<Button>(copies, "newButton").PerformClick();
        Check(!Find<TextBox>(copies, "codeBox").ReadOnly && Find<ComboBox>(copies, "stateCombo").Text == "AVAILABLE", "New copy must allow a new code and start AVAILABLE.");

        tabs.SelectedTab = tabs.TabPages["accountsTab"];
        Application.DoEvents();
        var accounts = Find<Library.Desktop.Controls.AccountsControl>(shell, "accountsControl");
        Find<ComboBox>(accounts, "roleCombo").Items.Add(Roles.Reader);
        Find<DataGridView>(accounts, "grid").DataSource = new List<UserDto> { new("r1", "SV001", "Bạn đọc mẫu", Roles.Reader, true) };
        Application.DoEvents();
        Check(Find<TextBox>(accounts, "userBox").ReadOnly && !Find<TextBox>(accounts, "passwordBox").Enabled, "Editing an account must protect username and creation password.");
        Find<Button>(accounts, "newButton").PerformClick();
        Check(!Find<TextBox>(accounts, "userBox").ReadOnly && Find<TextBox>(accounts, "passwordBox").Enabled, "New account must restore editable creation fields.");

        tabs.SelectedTab = tabs.TabPages["historyTab"];
        Application.DoEvents();
        var history = Find<Library.Desktop.Controls.HistoryControl>(shell, "historyControl");
        var from = Find<DateTimePicker>(history, "fromDate");
        var to = Find<DateTimePicker>(history, "toDate");
        Check(!from.Enabled && !to.Enabled, "Date controls must be disabled when date filtering is off.");
        Find<CheckBox>(history, "dateCheck").Checked = true;
        from.Value = DateTime.Today.AddDays(1);
        to.Value = DateTime.Today;
        Check(from.Enabled && to.Enabled && !Find<Button>(history, "searchButton").Enabled, "Reversed dates must prevent querying.");
        to.Value = from.Value.AddDays(1);
        Check(Find<Button>(history, "searchButton").Enabled, "Correcting dates must allow querying again.");

        tabs.SelectedTab = tabs.TabPages["metadataTab"];
        Application.DoEvents();
        var metadata = Find<Library.Desktop.Controls.MetadataControl>(shell, "metadataControl");
        Check(!Find<Button>(metadata, "saveButton").Enabled, "Empty metadata name must disable saving.");
        Find<TextBox>(metadata, "nameBox").Text = "Danh mục mới";
        Check(Find<Button>(metadata, "saveButton").Enabled, "Entering a metadata name must enable saving.");
        Find<ComboBox>(metadata, "kindCombo").SelectedIndex = 1;
        Check(Find<TextBox>(metadata, "nameBox").TextLength == 0 && !Find<Button>(metadata, "saveButton").Enabled, "Changing metadata kind must clear the old editor.");

        tabs.SelectedTab = tabs.TabPages["policyTab"];
        Application.DoEvents();
        var policy = Find<Library.Desktop.Controls.PolicyControl>(shell, "policyControl");
        Find<NumericUpDown>(policy, "daysBox").Value = 21;
        Check(Find<Label>(policy, "statusLabel").Text.Contains("21 ngày"), "Changing policy days must update its preview.");
        Check(shell.Text.Contains("Chính sách"), "Selecting a tab must update the shell title.");

        // Read the framework's actual default events, rather than assuming every double-click means Click.
        var controls = AllControls(shell).Where(c => !string.IsNullOrEmpty(c.Name) && c is TextBox or Button or ComboBox or CheckBox or NumericUpDown or DateTimePicker or CheckedListBox or DataGridView or TabControl);
        foreach (var control in controls)
        {
            var defaultEvent = System.ComponentModel.TypeDescriptor.GetDefaultEvent(control);
            Console.WriteLine($"DESIGNER: {control.Parent?.GetType().Name}.{control.Name} -> {defaultEvent?.Name}");
        }
        Console.WriteLine("PASS: All seven editors respond to input, selection, filters, new-record actions and policy changes without API access.");
        shell.Close();
    }

    private static IEnumerable<Control> AllControls(Control root)
    {
        foreach (Control child in root.Controls)
        {
            yield return child;
            foreach (var descendant in AllControls(child)) yield return descendant;
        }
    }
    private static void Check(bool valid, string message)
    {
        if (!valid) throw new InvalidOperationException(message);
    }
}
