using Android;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Views;
using Android.Widget;
using System.Text.Json.Nodes;

namespace hephaestus_android;

[Activity(Label = "Abrir ticket", Exported = false)]
public class QuickTicketActivity : Activity
{
    const int CameraPermissionRequest = 4101;
    const int CameraCaptureRequest = 4102;
    readonly List<Android.Net.Uri> photoUris = [];
    Android.Net.Uri? pendingPhotoUri;

    EditText title = null!;
    EditText equipment = null!;
    EditText description = null!;
    Spinner location = null!;
    Spinner priority = null!;
    RadioButton supportType = null!;
    ImageView photoPreview = null!;
    View photoActions = null!;
    TextView photoCount = null!;
    TextView formError = null!;

    protected override void OnCreate(Bundle? state)
    {
        base.OnCreate(state);
        if (!SessionStore.IsSignedIn(this))
        {
            StartActivity(new Intent(this, typeof(MainActivity)));
            Finish();
            return;
        }

        SetContentView(Resource.Layout.activity_quick_ticket);
        title = FindViewById<EditText>(Resource.Id.ticketTitle)!;
        equipment = FindViewById<EditText>(Resource.Id.ticketEquipment)!;
        description = FindViewById<EditText>(Resource.Id.ticketDescription)!;
        location = FindViewById<Spinner>(Resource.Id.ticketLocation)!;
        priority = FindViewById<Spinner>(Resource.Id.ticketPriority)!;
        supportType = FindViewById<RadioButton>(Resource.Id.supportType)!;
        photoPreview = FindViewById<ImageView>(Resource.Id.photoPreview)!;
        photoActions = FindViewById<View>(Resource.Id.photoActions)!;
        photoCount = FindViewById<TextView>(Resource.Id.photoCount)!;
        formError = FindViewById<TextView>(Resource.Id.formError)!;

        FindViewById<TextView>(Resource.Id.headerInitials)!.Text = SessionStore.Initials(this);
        ConfigureSpinner(location, ["Selecione uma localidade", "Lisboa", "Porto", "Remoto", "Outra localização"]);
        RefreshPriorities();
        FindViewById<RadioGroup>(Resource.Id.ticketTypeGroup)!.CheckedChange += (_, _) => RefreshPriorities();
        title.TextChanged += (_, _) => FindViewById<TextView>(Resource.Id.titleCounter)!.Text = $"{title.Text?.Length ?? 0}/200";
        FindViewById<Button>(Resource.Id.capturePhotoButton)!.Click += (_, _) => RequestCamera();
        FindViewById<Button>(Resource.Id.removePhotosButton)!.Click += (_, _) => RemovePhotos();
        FindViewById<Button>(Resource.Id.createTicketButton)!.Click += (_, _) => CreateTicket();
        BottomNavigation.Setup(this, NavigationTab.Tickets);
    }

    void ConfigureSpinner(Spinner spinner, string[] values, int selected = 0)
    {
        var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, values);
        adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
        spinner.Adapter = adapter;
        spinner.SetSelection(selected);
    }

    void RefreshPriorities()
    {
        var values = supportType.Checked
            ? new[] { "P1 · Crítico", "P2 · Alto", "P3 · Médio", "P4 · Baixo" }
            : new[] { "P1 · Urgente", "P2 · Prioritário", "P3 · Normal", "P4 · Flexível" };
        ConfigureSpinner(priority, values, 2);
    }

    void RequestCamera()
    {
        var permissions = new List<string>();
        if (CheckSelfPermission(Manifest.Permission.Camera) != Permission.Granted)
            permissions.Add(Manifest.Permission.Camera);
        if (Build.VERSION.SdkInt <= BuildVersionCodes.P && CheckSelfPermission(Manifest.Permission.WriteExternalStorage) != Permission.Granted)
            permissions.Add(Manifest.Permission.WriteExternalStorage);

        if (permissions.Count > 0)
            RequestPermissions([.. permissions], CameraPermissionRequest);
        else
            OpenCamera();
    }

    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
    {
        base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        if (requestCode != CameraPermissionRequest) return;
        if (grantResults.Length > 0 && grantResults.All(result => result == Permission.Granted))
            OpenCamera();
        else
            ShowError("Autorize o acesso à câmara para anexar fotografias.");
    }

    void OpenCamera()
    {
        var intent = new Intent(MediaStore.ActionImageCapture);
        if (intent.ResolveActivity(PackageManager!) is null)
        {
            ShowError("Não foi encontrada uma aplicação de câmara neste dispositivo.");
            return;
        }

        var values = new ContentValues();
        values.Put("_display_name", $"hephaestus_{DateTime.UtcNow:yyyyMMdd_HHmmss}.jpg");
        values.Put("mime_type", "image/jpeg");
        if (Build.VERSION.SdkInt >= BuildVersionCodes.Q)
            values.Put("relative_path", "Pictures/Hephaestus");
        pendingPhotoUri = ContentResolver!.Insert(MediaStore.Images.Media.ExternalContentUri!, values);
        if (pendingPhotoUri is null)
        {
            ShowError("Não foi possível preparar o ficheiro da fotografia.");
            return;
        }

        intent.PutExtra(MediaStore.ExtraOutput, pendingPhotoUri);
        intent.AddFlags(ActivityFlags.GrantWriteUriPermission | ActivityFlags.GrantReadUriPermission);
        StartActivityForResult(intent, CameraCaptureRequest);
    }

    protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
    {
        base.OnActivityResult(requestCode, resultCode, data);
        if (requestCode != CameraCaptureRequest || pendingPhotoUri is null) return;
        if (resultCode == Result.Ok)
        {
            photoUris.Add(pendingPhotoUri);
            photoPreview.SetImageURI(null);
            photoPreview.SetImageURI(pendingPhotoUri);
            photoPreview.Visibility = ViewStates.Visible;
            photoActions.Visibility = ViewStates.Visible;
            photoCount.Text = photoUris.Count == 1 ? "1 fotografia anexada" : $"{photoUris.Count} fotografias anexadas";
            formError.Visibility = ViewStates.Gone;
        }
        else
        {
            ContentResolver?.Delete(pendingPhotoUri, null, null);
        }
        pendingPhotoUri = null;
    }

    void RemovePhotos()
    {
        foreach (var uri in photoUris)
            ContentResolver?.Delete(uri, null, null);
        photoUris.Clear();
        photoPreview.SetImageDrawable(null);
        photoPreview.Visibility = ViewStates.Gone;
        photoActions.Visibility = ViewStates.Gone;
    }

    void CreateTicket()
    {
        title.Error = string.IsNullOrWhiteSpace(title.Text) ? "Indique um título." : null;
        equipment.Error = string.IsNullOrWhiteSpace(equipment.Text) ? "Identifique o equipamento." : null;
        description.Error = string.IsNullOrWhiteSpace(description.Text) ? "Descreva o pedido." : null;
        if (title.Error is not null || equipment.Error is not null || description.Error is not null || location.SelectedItemPosition == 0)
        {
            ShowError(location.SelectedItemPosition == 0 ? "Selecione a localidade e preencha os campos obrigatórios." : "Preencha os campos obrigatórios.");
            return;
        }

        var reference = $"{(supportType.Checked ? "SUP" : "SVC")}{DateTime.UtcNow:MMddHHmm}";
        var draft = new JsonObject
        {
            ["ReferenceCode"] = reference,
            ["TicketType"] = supportType.Checked ? "SUP" : "SVC",
            ["Title"] = title.Text!.Trim(),
            ["Description"] = description.Text!.Trim(),
            ["Equipment"] = equipment.Text!.Trim(),
            ["Location"] = location.SelectedItem!.ToString(),
            ["Priority"] = priority.SelectedItem!.ToString()![..2],
            ["Status"] = "Open",
            ["CreatedAt"] = DateTime.UtcNow,
            ["Photos"] = new JsonArray(photoUris.Select(uri => JsonValue.Create(uri.ToString())).ToArray())
        };
        var preferences = GetSharedPreferences("hephaestus_tickets", FileCreationMode.Private)!;
        var editor = preferences.Edit()!;
        editor.PutString(reference, draft.ToJsonString());
        editor.Apply();

        var dialog = new AlertDialog.Builder(this);
        dialog.SetTitle("Ticket criado");
        dialog.SetMessage($"{reference} foi registado com o estado Open e {photoUris.Count} fotografia(s).");
        dialog.SetPositiveButton("Concluir", (_, _) => ClearForm());
        dialog.SetCancelable(false);
        dialog.Show();
    }

    void ClearForm()
    {
        title.Text = "";
        equipment.Text = "";
        description.Text = "";
        location.SetSelection(0);
        supportType.Checked = true;
        photoUris.Clear();
        photoPreview.SetImageDrawable(null);
        photoPreview.Visibility = ViewStates.Gone;
        photoActions.Visibility = ViewStates.Gone;
        formError.Visibility = ViewStates.Gone;
    }

    void ShowError(string message)
    {
        formError.Text = message;
        formError.Visibility = ViewStates.Visible;
    }
}
