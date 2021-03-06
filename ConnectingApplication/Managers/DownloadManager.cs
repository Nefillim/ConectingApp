﻿using Assets.ConectingApp.ConnectingApplication.Managers;
using Assets.ConectingApp.ConnectingApplication.Managers.PathManagerImpls;
using Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectingApplication.Managers
{
    public class DownloadManager 
    {
		
        private Queue<Flag> saveQueue;
        private static int iteratorPosition;


        [Obsolete("Don't use outside the ConnectingApp.")]
        public DownloadManager()
        {
            saveQueue = new Queue<Flag>();
        }


        public void EnqueueFlag(Flag flag)
        {
            saveQueue.Enqueue(flag);
        }

        public void SetNewIteratorPosition()
        {
            iteratorPosition = saveQueue.Count;
        }

		public void Download(int type)
		{
			using (FileStream stream = new FileStream($"{PathManager.GetPathToSaveFilesDirectory()}{DefaultValues.PLAYER_ID}{type}", FileMode.Open))
			{
				var player = ConnectingAppManager.CharacterManager.GetPlayer();
#pragma warning disable CS0618 // Type or member is obsolete
				ConnectingAppManager.SaveMode = true;
#pragma warning restore CS0618 // Type or member is obsolete
				using (BinaryReader reader = new BinaryReader(stream))
				{
					iteratorPosition = reader.ReadInt32();
					int iterationsCount = reader.ReadInt32() - iteratorPosition;
					for (int i = 0; i < iterationsCount; i++)
					{
						Flag flag = new Flag();
						flag.Download(reader);
						saveQueue.Enqueue(flag);
						ConnectingAppManager.FlagManager.SetFlag(flag.name, flag.value);
					}
#pragma warning disable CS0618 // Type or member is obsolete
					ConnectingAppManager.SaveMode = false;
#pragma warning restore CS0618 // Type or member is obsolete
					for (int i = 0; i < iterationsCount; i++)
					{
						Flag flag = new Flag();
						flag.Download(reader);
						saveQueue.Enqueue(flag);
						ConnectingAppManager.FlagManager.SetFlag(flag.name, flag.value);
					}
					int contactsCount = reader.ReadInt32();
					for (int i = 0; i < contactsCount; i++)
					{
						player.AddContact(reader.ReadString(), Characters.Player.ContactType.Phone);
					}
					player.DownloadMessageHistory(reader);
				}
				//TODO: smth else to download?
			}
		}

		public void Save(int type)
		{

			using (FileStream stream = new FileStream($"{PathManager.GetPathToSaveFilesDirectory()}{DefaultValues.PLAYER_ID}{type}", FileMode.OpenOrCreate))
			{
				using (BinaryWriter writer = new BinaryWriter(stream))
				{
					writer.Write(iteratorPosition);
					writer.Write(saveQueue.Count);
					while (saveQueue.Count > 0)
					{
						writer.Write(saveQueue.Dequeue().name);
						writer.Write(saveQueue.Dequeue().value);
					}
					//TODO: smth else to save?
					var playerContacts = ConnectingAppManager.CharacterManager.GetPlayer().GetContacts(Characters.Player.ContactType.Phone);
					writer.Write(playerContacts.Count);
					foreach (string contact in playerContacts)
					{
						writer.Write(contact);
					}
					ConnectingAppManager.CharacterManager.GetPlayer().SaveMessageHistory(writer);
				}
			}
		}
    }
}
