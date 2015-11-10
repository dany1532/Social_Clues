using UnityEngine;
using MySql.Data;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;

public class MySQL : MonoBehaviour
{
    // In truth, the only things you want to save to the database are dynamic objects
    // static objects in the scene will always exist, so make sure to set your Tag 
    // based on the documentation for this demo

    // values to match the database columns
    string ID, Name, levelname, objectType;
    float posx, posy, posz, tranx, trany, tranz;
//glados6.isi.edu:5552
	// SocialClues : GPfighton1!
    bool saving = false;
    bool loading = false;
    // MySQL instance specific items
    string constr = "Server=glados6.isi.edu;Database=SocialClues;User ID=SocialClues;Password=SocialClues@USC;Pooling=true";
    // connection object
    MySqlConnection con = null;
    // command object
    MySqlCommand cmd = null;
    // reader object
    MySqlDataReader rdr = null;
    // object collection array
    GameObject[] bodies;
    // object definitions
    public struct data
    {
        public int UID;
        public string ID, Name, levelname, objectType;
        public float posx, posy, posz, tranx, trany, tranz;
    }
    // collection container
    List<data> _GameItems;
    void Awake()
    {
        try
        {
            // setup the connection element
            con = new MySqlConnection(constr);

            // lets see if we can open the connection
            con.Open();
            Debug.Log("Connection State: " + con.State);
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }

    }

    void OnApplicationQuit()
    {
        Debug.Log("killing con");
        if (con != null)
        {
            if (con.State.ToString() != "Closed")
                con.Close();
            con.Dispose();
        }
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    // gui event like a button, etc
    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 70, 50, 30), "Save") && !saving)
        {
            saving = true;
            // first lets clean out the databae
            DeleteEntries();
            // now lets save the scene information
            InsertEntries();
            // you could also use the update if you know the ID of the item already saved

            saving = false;
        }
        if (GUI.Button(new Rect(10, 110, 50, 30), "Load") && !loading)
        {
            loading = true;
            // lets read the items from the database
            ReadEntries();
            // now display what is known about them to our log
            LogGameItems();
            loading = false;
        }
    }

    // Insert new entries into the table
    void InsertEntries()
    {
        prepData();
        string query = string.Empty;
        // Error trapping in the simplest form
        try
        {
            query = "INSERT INTO User (idUser, UserName) VALUES (?ID, ?UserName)";
            if (con.State.ToString() != "Open")
                con.Open();
            using (con)
            {
                foreach (data itm in _GameItems)
                {
                    using (cmd = new MySqlCommand(query, con))
                    {
                        MySqlParameter oParam = cmd.Parameters.Add("?ID", MySqlDbType.VarChar);
                        oParam.Value = itm.ID;
                        MySqlParameter oParam1 = cmd.Parameters.Add("?UserName", MySqlDbType.VarChar);
                        oParam1.Value = itm.Name;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }
        finally
        {
        }
    }

    // Update existing entries in the table based on the iddemo_table
    void UpdateEntries()
    {
        prepData();
        string query = string.Empty;
        // Error trapping in the simplest form
        try
        {
            query = "UPDATE demo_table SET ID=?ID, Name=?Name, levelname=?levelname, objectType=?objectType, posx=?posx, posy=?posy, posz=?posz, tranx=?tranx, trany=?trany, tranz=?tranz WHERE iddemo_table=?UID";
            if (con.State.ToString() != "Open")
                con.Open();
            using (con)
            {
                foreach (data itm in _GameItems)
                {
                    using (cmd = new MySqlCommand(query, con))
                    {
                        MySqlParameter oParam = cmd.Parameters.Add("?ID", MySqlDbType.VarChar);
                        oParam.Value = itm.ID;
                        MySqlParameter oParam1 = cmd.Parameters.Add("?Name", MySqlDbType.VarChar);
                        oParam1.Value = itm.Name;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }
        finally
        {
        }
    }

    // Delete entries from the table
    void DeleteEntries()
    {
        string query = string.Empty;
        // Error trapping in the simplest form
        try
        {
            // optimally you will know which items you want to delete from the database
            // using the following code and the record ID, you can delete the entry
            //-----------------------------------------------------------------------
            // query = "DELETE FROM demo_table WHERE iddemo_table=?UID";
            // MySqlParameter oParam = cmd.Parameters.Add("?UID", MySqlDbType.Int32);
            // oParam.Value = 0;
            //-----------------------------------------------------------------------
            query = "DELETE FROM demo_table WHERE iddemo_table";
            if (con.State.ToString() != "Open")
                con.Open();
            using (con)
            {
                using (cmd = new MySqlCommand(query, con))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }
        finally
        {
        }
    }

    // Read all entries from the table
    void ReadEntries()
    {
        string query = string.Empty;
        if (_GameItems == null)
            _GameItems = new List<data>();
        if (_GameItems.Count > 0)
            _GameItems.Clear();
        // Error trapping in the simplest form
        try
        {
            query = "SELECT * FROM User";
            if (con.State.ToString() != "Open")
                con.Open();
            using (con)
            {
                using (cmd = new MySqlCommand(query, con))
                {
                    rdr = cmd.ExecuteReader();
                    if (rdr.HasRows)
                        while (rdr.Read())
                        {
                            data itm = new data();
                            itm.UID = int.Parse(rdr["iddemo_table"].ToString());
                            itm.ID = rdr["ID"].ToString();
                            itm.levelname = rdr["levelname"].ToString();
                            itm.Name = rdr["Name"].ToString();
                            itm.objectType = rdr["objectType"].ToString();
                            itm.posx = float.Parse(rdr["posx"].ToString());
                            itm.posy = float.Parse(rdr["posy"].ToString());
                            itm.posz = float.Parse(rdr["posz"].ToString());
                            itm.tranx = float.Parse(rdr["tranx"].ToString());
                            itm.trany = float.Parse(rdr["trany"].ToString());
                            itm.tranz = float.Parse(rdr["tranz"].ToString());
                            _GameItems.Add(itm);
                        }
                    rdr.Dispose();
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }
        finally
        {
        }
    }

    /// <summary>
    /// Lets show what was read back to the log window
    /// </summary>
    void LogGameItems()
    {
        if (_GameItems != null)
        {
            if (_GameItems.Count > 0)
            {
                foreach (data itm in _GameItems)
                {
                    Debug.Log("UID: " + itm.UID);
                    Debug.Log("ID: " + itm.ID);
                    Debug.Log("levelname: " + itm.levelname);
                    Debug.Log("Name: " + itm.Name);
                    Debug.Log("objectType: " + itm.objectType);
                    Debug.Log("posx: " + itm.posx);
                    Debug.Log("posy: " + itm.posy);
                    Debug.Log("posz: " + itm.posz);
                    Debug.Log("tranx: " + itm.tranx);
                    Debug.Log("trany: " + itm.trany);
                    Debug.Log("tranz: " + itm.tranz);
                }
            }
        }
    }

    /// <summary>
    /// This method prepares the data to be saved into our database
    /// 
    /// </summary>
    void prepData()
    {
        bodies = GameObject.FindGameObjectsWithTag("Savable");
        _GameItems = new List<data>();
        data itm;
        foreach (GameObject body in bodies)
        {
            itm = new data();
            itm.ID = body.name + "_" + body.GetInstanceID();
            itm.Name = body.name;
            itm.levelname = Application.loadedLevelName;
            itm.objectType = body.name.Replace("(Clone)", "");
            itm.posx = body.transform.position.x;
            itm.posy = body.transform.position.y;
            itm.posz = body.transform.position.z;
            itm.tranx = body.transform.rotation.x;
            itm.trany = body.transform.rotation.y;
            itm.tranz = body.transform.rotation.z;
            _GameItems.Add(itm);
        }
        Debug.Log("Items in collection: " + _GameItems.Count);
    }
}