using System;
using System.Collections.Generic;
using ExitGames.Client.Photon.Chat;
using UnityEngine;

public class DNChatGUI : MonoBehaviour, IChatClientListener {
	// Chat variables
	public string ChatAppId;
	public string[] ChannelsToJoinOnConnect;
	public int HistoryLengthToFetch;
	public bool DemoPublishOnSubscribe;
	
	public string UserName {get;set;}
	private ChatChannel selectedChannel;
	private string selectedChannelName;
	private int selectedChannelIndex = 0;
	bool doingPrivateChat;
	
	public ChatClient chatClient;
	
	// GUI Display Configuration
	public Rect ChatWindow = new Rect(0, 0, 250, 300);
	public bool IsVisible = true;
//	public bool publicVisible = true;
//	public bool partyVisible = false;
//	public bool guildVisible = false;
	public bool AlignBottom = true;
	public bool FullScreen = false;
	
	private string inputLine = "";
	private string userIdInput = "";
	private Vector2 scrollPos = Vector2.zero;
	private static string WelcomeText = "Welcome to Darknet. Type '\\help' for more info.\n";
	private static string HelpText = "";
	
	// public void StartDN() {
	public void Start() {	
		// This must run in the background or it will drop connection if not focused.
		Application.runInBackground = true;
		
		if (string.IsNullOrEmpty(this.UserName)) {
			// Made up username.
			this.UserName = "user" + Environment.TickCount%99; 
			// Set username to be the username of the user.
			// this.Username = ?
		}
		
		chatClient = new ChatClient(this);
		chatClient.Connect(ChatAppId, "1.0", this.UserName, null);
		
		if (this.AlignBottom) {
			this.ChatWindow.y = Screen.height - this.ChatWindow.height;
		}
		if (this.FullScreen) {
			this.ChatWindow.x = 0;
			this.ChatWindow.y = 0;
			this.ChatWindow.width = Screen.width;
			this.ChatWindow.height = Screen.height;			
		}
		
		Debug.Log(this.UserName);
	}
	
	// Disconnect all photon chat connections in OnApplicationQuit.
	public void OnApplicationQuit() {
		if (this.chatClient != null) {
			this.chatClient.Disconnect();
		}
	}
	
	public void Update() {
		if (this.chatClient != null) {
			// Make sure to call this regularly. It limits effort internally.
			// Calling often is okay.
			this.chatClient.Service();
		}
	}
	
	public void OnGUI() {
		if (!this.IsVisible) {
			return;
		}
		GUI.skin.label.wordWrap = true;
		
		if (Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.KeypadEnter || Event.current.keyCode == KeyCode.Return)) {
			if ("ChatInput".Equals(GUI.GetNameOfFocusedControl())) {
				// Focus on input -> submit
				GuiSendsMsg();
				// Show the now modified list would result in an error. To avoid this, we skip a frame.
				return;
			}
			else {
				// Assign focus to input.
				GUI.FocusControl("ChatInput");
			}
		}
		
		GUI.SetNextControlName("");
		GUILayout.BeginArea(this.ChatWindow);
		
		GUILayout.FlexibleSpace();
		
		if (this.chatClient.State != ChatState.ConnectedToFrontEnd) {
			GUILayout.Label("Not in chat yet.");
		}
		else {
			// This could be cached.
			List<string> channels = new List<string>(this.chatClient.PublicChannels.Keys);
			int countOfPublicChannels = channels.Count;
			channels.AddRange(this.chatClient.PrivateChannels.Keys);
			
			if (channels.Count > 0) {
				int previouslySelectedChannelIndex = this.selectedChannelIndex;
				int channelIndex = channels.IndexOf(this.selectedChannelName);
				this.selectedChannelIndex = (channelIndex >= 0) ? channelIndex : 0;
				
				this.selectedChannelIndex = GUILayout.Toolbar(this.selectedChannelIndex, channels.ToArray(), GUILayout.ExpandWidth(false));
				this.scrollPos = GUILayout.BeginScrollView(this.scrollPos);
				
				this.doingPrivateChat = (this.selectedChannelIndex >= countOfPublicChannels);
				this.selectedChannelName = channels[this.selectedChannelIndex];
				
				if (this.selectedChannelIndex != previouslySelectedChannelIndex) {
					// Change channel -> Scroll Down. If private, pre-fill to field with target user's name.
					this.scrollPos.y = float.MaxValue;
					if (this.doingPrivateChat) {
						string[] pieces = this.selectedChannelName.Split(new char[] {':'}, 3);
						this.userIdInput = pieces[1];
					}
				}
				
				GUILayout.Label(DNChatGUI.WelcomeText);
				
				if (this.chatClient.TryGetChannel(selectedChannelName, this.doingPrivateChat, out this.selectedChannel)) {
					for (int i = 0; i < this.selectedChannel.Messages.Count; i++) {
						string sender = this.selectedChannel.Senders[i];
						object message = this.selectedChannel.Messages[i];
						GUILayout.Label(string.Format("{0}: {1}", sender, message));
					}
				}
				
				GUILayout.EndScrollView();
			}
		}
		
		GUILayout.BeginHorizontal();
		
		if (doingPrivateChat) {
			GUILayout.Label("to:", GUILayout.ExpandWidth(false));
			GUI.SetNextControlName("WhisperTo");
			this.userIdInput = GUILayout.TextField(this.userIdInput, GUILayout.MinWidth(100), GUILayout.ExpandWidth(false));
			string focussed = GUI.GetNameOfFocusedControl();
			if (focussed.Equals("WhisperTo")) {
				if (this.userIdInput.Equals("username")) {
					this.userIdInput = "";
				}
			}
			else if (string.IsNullOrEmpty(this.userIdInput)) {
				this.userIdInput = "username";
			}
		}
		
		GUI.SetNextControlName("ChatInput");
		inputLine = GUILayout.TextField(inputLine);
		if (GUILayout.Button("Send", GUILayout.ExpandWidth(false))) {
			GuiSendsMsg();
		}
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}
	
	private void GuiSendsMsg() {
		if (string.IsNullOrEmpty(this.inputLine)) {
			GUI.FocusControl("");
			return;
		}
		
		if (this.inputLine[0].Equals(('\\'))) {
			string[] tokens = this.inputLine.Split(new char[] {' '}, 2);
			if (tokens[0].Equals("\\help")){
				this.PostHelpToCurrentChannel();
			}
			if (tokens[0].Equals("\\clear")) {
				if (this.doingPrivateChat) {
					this.chatClient.PrivateChannels.Remove(this.selectedChannelName);
				}
				else {
					ChatChannel channel;
					if (this.chatClient.TryGetChannel(this.selectedChannelName, this.doingPrivateChat, out channel)) {
						channel.ClearMessages();
					}
				}
			}
			else if ((tokens[0].Equals("\\w") || tokens[0].Equals("\\msg")) && !string.IsNullOrEmpty(tokens[1])) {
				string[] subtokens = tokens[1].Split(new char[] {' ', ','}, 2);
				string targetuser = subtokens[0];
				string message = subtokens[1];
				this.chatClient.SendPrivateMessage(targetuser, message);
			}
		}
		else {
			if (this.doingPrivateChat) {
				this.chatClient.SendPrivateMessage(this.userIdInput, this.inputLine);
			}
			else {
				Debug.Log("Channel Name: " + this.selectedChannelName);
				Debug.Log("Input Line: " + this.inputLine);
				this.chatClient.PublishMessage(this.selectedChannelName, this.inputLine);
			}
		}
		
		this.inputLine = "";
		GUI.FocusControl("");
	}
	
	private void PostHelpToCurrentChannel() {
		ChatChannel channelForHelp = this.selectedChannel;
		if (channelForHelp != null) {
			channelForHelp.Add("info", DNChatGUI.HelpText);
		}
		else {
			Debug.LogError("No channel for help.");
		}
	}
	
	public void OnConnected() {
		if (this.ChannelsToJoinOnConnect != null && this.ChannelsToJoinOnConnect.Length > 0) {
			this.chatClient.Subscribe(this.ChannelsToJoinOnConnect, this.HistoryLengthToFetch);
		}
		// Add friends?
		// this.chatClient.AddFriend(new str[] {"tobi", "ilya"});
		this.chatClient.SetOnlineStatus(ChatUserStatus.Online);
	}
	
	public void OnDisconnected() {
		Debug.Log(UserName + " has left the room.");
	}
	
	public void OnChatStateChange(ChatState state) {
		// use OnConnected() and OnDisconnected()
		// This method might become more useful in the future, when more complex states are being used.
	}
	
	public void OnSubscribed(string[] channels, bool[] results) {
		// Demo can automatically send "hi" to subscribed channels.
		// In games, you should only send user's input.
		if (this.DemoPublishOnSubscribe) {
			foreach (string channel in channels) {
				// You don't have to send a message on join.
				this.chatClient.PublishMessage(channel, " has subscribed.");
			}
		}
	}
	
	public void OnUnsubscribed(string[] channels) {
		Debug.Log(UserName + " has unsubscribed.");
	}
	
	public void OnGetMessages(string channelName, string[] senders, object[] messages) {
		if (channelName.Equals(this.selectedChannelName)) {
			this.scrollPos.y = float.MaxValue;
		}
	}
	
	public void OnPrivateMessage(string sender, object message, string channelName) {
		// As the ChatClient is buffering the messages for you, this GUI doesn't need to do anything here.
		// You also get messages that you sent yourself. In that case, the channelName is determined by the target of your message.
	}
	
	public void OnStatusUpdate(string user, int status, bool gotMessage, object message) {
		// This is how you get the status updates of friends.
		// Demo simply adds status updates to the currently shown chat.
		// You could buffer them or use them any other way, too.
		
		ChatChannel activeChannel = this.selectedChannel;
		if (activeChannel != null) {
			activeChannel.Add("info", string.Format("{0} is {1}. Msg:{2}", user, status, message));
		}
		Debug.LogWarning("status: " + string.Format("{0} is {1}. Msg:{2}", user, status, message));
	}
	
	public void InitializePartyChat() {

	}
	
	public void InitializeGuildChat() {
		
	}
}