using UnityEngine;
using System.Collections;
using System.IO;
 
// originally found in: http://forum.unity3d.com/threads/35617-TextManager-Localization-Script
 
/// <summary>
/// TextManager
/// 
/// Reads PO files in the Assets\Resources\Languages directory into a Hashtable.
/// Look for "PO editors" as that's a standard for translating software.
/// 
/// Example:
/// 
/// load the language file:
///   TextManager.LoadLanguage("helptext-pt-br");
/// 
/// which has to contain at least two lines as such:
///   msgid "HELLO WORLD"
///   msgstr "OLA MUNDO"
/// 
/// then we can retrieve the text by calling:
///   TextManager.GetText("HELLO WORLD");
/// </summary>
public class TextLocalizationManager : MonoBehaviour {
 
	private static TextLocalizationManager instance;// = new TextLocalizationManager();
	private Hashtable textTable;

	//we can set both as properties in unity editor
	public string comment = "#";
	public string separator = "=";
	//we can set both as properties in unity editor
	public string languageFilePrefix = "Language_";
	//public string languageFileSuffix = "";

	private const string DEFAULT_LANGUAGE_PREFIX = "en";

	//set the support languages array
	public string [] supportedLanguages;

	//private constructor
	
	private TextLocalizationManager() {
	
	}
	
	
	void Awake()
	{
		// Register the singleton
		if (Instance != null)
		{
			Debug.Log("Multiple instances of TextLocalizationManager!");
		}
		else {
			instance = this;
		}
		
	}
 
   
	public static TextLocalizationManager Instance 
	{
		get 
		{
			if (instance == null) 
			{
			
				instance = (TextLocalizationManager)FindObjectOfType(typeof(TextLocalizationManager));
				
				//none loaded?
				if(instance==null) {
				
				    //check in scripts hierarquy
					GameObject scripts = GameObject.FindGameObjectWithTag("Scripts");
					if(scripts!=null) {
						instance = scripts.GetComponent<TextLocalizationManager>();
					}
					else {
						// Because the TextManager is a component, we have to create a GameObject to attach it to.
						// Add the DynamicObjectManager component, and set it as the defaultCenter
						instance = (new GameObject("TextLocalizationManager")).AddComponent<TextLocalizationManager>();
					}
				
					
				}
				
				
    		}
			return instance;
		}
	}

	public bool LoadSystemLanguage(SystemLanguage language) {

	 if(language==null) {
		return LoadLanguage(DEFAULT_LANGUAGE_PREFIX);
	 }
	 else if(language == SystemLanguage.Spanish) {
	   return LoadLanguage("es");
	 }
	 else if(language == SystemLanguage.Portuguese) {
	   return LoadLanguage("pt");
	 }
	 else if(language == SystemLanguage.French) {
	   return LoadLanguage("fr");
	 }
	 else if(language == SystemLanguage.Russian) {
	   return LoadLanguage("ru");
	 }
	 else if(language == SystemLanguage.Chinese) {
	   return LoadLanguage("zh");
	 }
	 else if(language == SystemLanguage.Japanese) {
	   return LoadLanguage("jp");
	 }
	 else {
	 //english by default
	   return LoadLanguage(DEFAULT_LANGUAGE_PREFIX);
	 }

	}

	public bool LoadLanguage (string langPrefix)
	{
 
 
		if (langPrefix == null || supportedLanguages==null || supportedLanguages.Length == 0) {
			Debug.Log ("Loading default language.");
			textTable = null; // reset to default
			return false; // this means: call LoadLanguage with null to reset to default

		} else {
			bool exists = false;
			string lang = null;
			for(int i=0; i < supportedLanguages.Length; i++) {
				lang = supportedLanguages[i];
				if(lang.ToLower().Equals(langPrefix.ToLower()) ) {
					exists = true;
					break;
				}
			}
			//this is a supported languages
			if(exists) {
				string filename = languageFilePrefix + lang;
				return ParseLanguageFile(filename);
			}

			Debug.Log ("Unable to find a language match. Loading default language.");
			textTable = null; // reset to default
			return false;
		}


	}

	/**
	 * Parses the "properties" file
	 * */
	private bool ParseLanguageFile (string filename)
	{

 
		TextAsset textAsset = (TextAsset)Resources.Load (filename) as TextAsset;
		if (textAsset == null) {
			Debug.Log ("[TextManager] " + filename + " file not found.");
			return false;
		}
 
		if (textTable == null) {
			textTable = new Hashtable ();
		}
 
		textTable.Clear ();
 		
		StringReader reader = new StringReader (textAsset.text);

		string line;
		string key = null;
		string val = null;
		int indexSeparator = 0;

		while ((line = reader.ReadLine()) != null) {


			if (line.StartsWith (comment)) {
				//ignore since is a comment
				continue;
			} else {

				//parse it
				if (line.Contains (separator)) {

					indexSeparator = line.IndexOf (separator);
					key = line.Substring (0, indexSeparator );
					val = line.Substring (indexSeparator + 1);
				}

				if (key != null && val != null) {
					// TODO: add error handling here in case of duplicate keys
					if (!textTable.ContainsKey (key)) {

						textTable.Add (key, val);
					} else {
						Debug.Log ("Duplicated entry: " + key + " , ignoring it...");
					}
			    		

				} 
		    		
				key = val = null;

			}
		}

		reader.Close ();
		

		return true;
	}
 
 
	public string GetText (string key)
	{
		if (key != null && textTable != null)
		{
			if (textTable.ContainsKey(key))
			{
				string result = (string)textTable[key];
				if (result.Length > 0)
				{
					return result;
				}
			}
		}
		return key;
	}
}
