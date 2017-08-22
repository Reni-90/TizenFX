/*
 * Copyright (c) 2016 Samsung Electronics Co., Ltd All Rights Reserved
 *
 * Licensed under the Apache License, Version 2.0 (the License);
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an AS IS BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Tizen.Content.MediaContent
{
    /// <summary>
    /// Provides commands to manage playlists in the database.
    /// </summary>
    /// <seealso cref="Playlist"/>
    public class PlaylistCommand : MediaCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlaylistCommand"/> class with the specified <see cref="MediaDatabase"/>.
        /// </summary>
        /// <param name="database">A <see cref="MediaDatabase"/> that the commands run on.</param>
        /// <exception cref="ArgumentNullException"><paramref name="database"/> is null.</exception>
        /// <exception cref="ObjectDisposedException"><paramref name="database"/> has already been disposed of.</exception>
        public PlaylistCommand(MediaDatabase database) : base(database)
        {
        }

        /// <summary>
        /// Retrieves the number of playlists.
        /// </summary>
        /// <returns>The number of playlists.</returns>
        /// <exception cref="InvalidOperationException">The <see cref="MediaDatabase"/> is disconnected.</exception>
        /// <exception cref="ObjectDisposedException">The <see cref="MediaDatabase"/> has already been disposed of.</exception>
        /// <exception cref="MediaDatabaseException">An error occurred while executing the command.</exception>
        public int Count()
        {
            return Count(null);
        }

        /// <summary>
        /// Retrieves the number of playlists with <see cref="CountArguments"/>.
        /// </summary>
        /// <param name="arguments">The criteria to use to filter. This value can be null.</param>
        /// <returns>The number of playlists.</returns>
        /// <exception cref="InvalidOperationException">The <see cref="MediaDatabase"/> is disconnected.</exception>
        /// <exception cref="ObjectDisposedException">The <see cref="MediaDatabase"/> has already been disposed of.</exception>
        /// <exception cref="MediaDatabaseException">An error occurred while executing the command.</exception>
        public int Count(CountArguments arguments)
        {
            ValidateDatabase();

            return CommandHelper.Count(Interop.Playlist.GetPlaylistCount, arguments);
        }

        /// <summary>
        /// Retrieves the play order of the member.
        /// </summary>
        /// <param name="playlistId">The playlist id.</param>
        /// <param name="memberId">The member id of the playlist.</param>
        /// <returns>The <see cref="MediaDataReader{TRecord}"/> containing the results.</returns>
        /// <exception cref="InvalidOperationException">The <see cref="MediaDatabase"/> is disconnected.</exception>
        /// <exception cref="ObjectDisposedException">The <see cref="MediaDatabase"/> has already been disposed of.</exception>
        /// <exception cref="MediaDatabaseException">An error occurred while executing the command.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="playlistId"/> is less than or equal to zero.\n
        ///     -or-\n
        ///     <paramref name="memberId"/> is less than or equal to zero.
        /// </exception>
        public int GetPlayOrder(int playlistId, int memberId)
        {
            ValidateDatabase();

            if (playlistId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(playlistId), playlistId,
                    "Playlist id can't be less than or equal to zero.");
            }

            if (memberId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(memberId), memberId,
                    "Member id can't be less than or equal to zero.");
            }

            Interop.Playlist.GetPlayOrder(playlistId, memberId, out var order).ThrowIfError("Failed to query");

            return order;
        }

        /// <summary>
        /// Deletes a playlist from the database.
        /// </summary>
        /// <privilege>http://tizen.org/privilege/content.write</privilege>
        /// <param name="playlistId">The playlist id to delete.</param>
        /// <returns>true if the matched record was found and deleted, otherwise false.</returns>
        /// <exception cref="InvalidOperationException">The <see cref="MediaDatabase"/> is disconnected.</exception>
        /// <exception cref="ObjectDisposedException">The <see cref="MediaDatabase"/> has already been disposed of.</exception>
        /// <exception cref="MediaDatabaseException">An error occurred while executing the command.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="playlistId"/> is less than or equal to zero.</exception>
        /// <exception cref="UnauthorizedAccessException">The caller has no required privilege.</exception>
        public bool Delete(int playlistId)
        {
            ValidateDatabase();

            if (playlistId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(playlistId), playlistId,
                    "Playlist id can't be less than or equal to zero.");
            }

            if (Select(playlistId) == null)
            {
                return false;
            }

            CommandHelper.Delete(Interop.Playlist.Delete, playlistId);
            return true;
        }

        /// <summary>
        /// Inserts a playlist into the database from the specified m3u file.
        /// </summary>
        /// <remarks>
        ///     If you want to access internal storage, you should add privilege http://tizen.org/privilege/mediastorage.\n
        ///     If you want to access external storage, you should add privilege http://tizen.org/privilege/externalstorage.
        /// </remarks>
        /// <privilege>http://tizen.org/privilege/content.write</privilege>
        /// <privilege>http://tizen.org/privilege/mediastorage</privilege>
        /// <privilege>http://tizen.org/privilege/externalstorage</privilege>
        /// <param name="name">The name of playlist.</param>
        /// <param name="path">The path to a m3u file to import.</param>
        /// <returns>The <see cref="Playlist"/> instance that contains the record information inserted.</returns>
        /// <exception cref="InvalidOperationException">The <see cref="MediaDatabase"/> is disconnected.</exception>
        /// <exception cref="ObjectDisposedException">The <see cref="MediaDatabase"/> has already been disposed of.</exception>
        /// <exception cref="MediaDatabaseException">An error occurred while executing the command.</exception>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="name"/> is null.\n
        ///     -or-\n
        ///     <paramref name="path"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="name"/> is a zero-length string.\n
        ///     -or-\n
        ///     <paramref name="path"/> is a zero-length string, contains only white space.
        /// </exception>
        /// <exception cref="FileNotFoundException"><paramref name="path"/> does not exists.</exception>
        /// <exception cref="UnauthorizedAccessException">The caller has no required privilege.</exception>
        public Playlist InsertFromFile(string name, string path)
        {
            ValidateDatabase();

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (name.Length == 0)
            {
                throw new ArgumentException("Playlist name can't be an empty string.");
            }

            ValidationUtil.ValidateNotNullOrEmpty(path, nameof(path));

            if (File.Exists(path) == false)
            {
                throw new FileNotFoundException("The specified path does not exists.", path);
            }

            IntPtr handle = IntPtr.Zero;
            Interop.Playlist.ImportFromFile(path, name, out handle).ThrowIfError("Failed to insert");

            try
            {
                return new Playlist(handle);
            }
            finally
            {
                if (handle != IntPtr.Zero)
                {
                    Interop.Playlist.Destroy(handle);
                }
            }
        }
        /// <summary>
        /// Exports a playlist to a m3u file.
        /// </summary>
        /// <remarks>
        ///     If the file already exists in the file system, then it will be overwritten.\n
        ///     \n
        ///     If you want to access internal storage, you should add privilege http://tizen.org/privilege/mediastorage.\n
        ///     If you want to access external storage, you should add privilege http://tizen.org/privilege/externalstorage.
        /// </remarks>
        /// <privilege>http://tizen.org/privilege/mediastorage</privilege>
        /// <privilege>http://tizen.org/privilege/externalstorage</privilege>
        /// <param name="playlistId">The playlist id to export.</param>
        /// <param name="path">The path to a m3u file.</param>
        /// <exception cref="InvalidOperationException">The <see cref="MediaDatabase"/> is disconnected.</exception>
        /// <exception cref="ObjectDisposedException">The <see cref="MediaDatabase"/> has already been disposed of.</exception>
        /// <exception cref="MediaDatabaseException">An error occurred while executing the command.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="path"/> is a zero-length string, contains only white space.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="playlistId"/> is less than or equal to zero.</exception>
        /// <exception cref="RecordNotFoundException">No matching playlist exists.</exception>
        /// <exception cref="UnauthorizedAccessException">The caller has no required privilege.</exception>
        public void ExportToFile(int playlistId, string path)
        {
            ValidateDatabase();

            ValidationUtil.ValidateNotNullOrEmpty(path, nameof(path));

            if (playlistId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(playlistId), playlistId,
                    "Playlist id can't be less than or equal to zero.");
            }

            IntPtr handle = IntPtr.Zero;
            try
            {
                Interop.Playlist.GetPlaylistFromDb(playlistId, out handle).ThrowIfError("Failed to query");

                if (handle == IntPtr.Zero)
                {
                    throw new RecordNotFoundException("No matching playlist exists.");
                }

                Interop.Playlist.ExportToFile(handle, path).ThrowIfError("Failed to export");
            }
            finally
            {
                if (handle != IntPtr.Zero)
                {
                    Interop.Playlist.Destroy(handle);
                }
            }
        }

        /// <summary>
        /// Inserts a playlist into the database with the specified name.
        /// </summary>
        /// <privilege>http://tizen.org/privilege/content.write</privilege>
        /// <param name="name">The name of playlist.</param>
        /// <returns>The <see cref="Playlist"/> instance that contains the record information inserted.</returns>
        /// <exception cref="InvalidOperationException">The <see cref="MediaDatabase"/> is disconnected.</exception>
        /// <exception cref="ObjectDisposedException">The <see cref="MediaDatabase"/> has already been disposed of.</exception>
        /// <exception cref="MediaDatabaseException">An error occurred while executing the command.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="name"/> is a zero-length string.</exception>
        /// <exception cref="UnauthorizedAccessException">The caller has no required privilege.</exception>
        public Playlist Insert(string name)
        {
            return Insert(name, null);
        }

        /// <summary>
        /// Inserts a playlist into the database with the specified name and thumbnail path.
        /// </summary>
        /// <privilege>http://tizen.org/privilege/content.write</privilege>
        /// <param name="name">The name of playlist.</param>
        /// <param name="thumbnailPath">The path of thumbnail for playlist. This value can be null.</param>
        /// <returns>The <see cref="Playlist"/> instance that contains the record information inserted.</returns>
        /// <exception cref="InvalidOperationException">The <see cref="MediaDatabase"/> is disconnected.</exception>
        /// <exception cref="ObjectDisposedException">The <see cref="MediaDatabase"/> has already been disposed of.</exception>
        /// <exception cref="MediaDatabaseException">An error occurred while executing the command.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="name"/> is a zero-length string.</exception>
        /// <exception cref="UnauthorizedAccessException">The caller has no required privilege.</exception>
        public Playlist Insert(string name, string thumbnailPath)
        {
            ValidateDatabase();

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (name.Length == 0)
            {
                throw new ArgumentException("Playlist name can't be an empty string.");
            }

            IntPtr handle = IntPtr.Zero;
            Interop.Playlist.Create(out handle).ThrowIfError("Failed to insert");

            try
            {
                Interop.Playlist.SetName(handle, name).ThrowIfError("Failed to insert");

                if (thumbnailPath != null)
                {
                    Interop.Playlist.SetThumbnailPath(handle, thumbnailPath).ThrowIfError("Failed to insert");
                }

                Interop.Playlist.Insert(handle).ThrowIfError("Failed to insert");
                return new Playlist(handle);
            }
            finally
            {
                if (handle != IntPtr.Zero)
                {
                    Interop.Playlist.Destroy(handle);
                }
            }
        }

        /// <summary>
        /// Retrieves the playlists.
        /// </summary>
        /// <returns>The <see cref="MediaDataReader{TRecord}"/> containing the results.</returns>
        /// <exception cref="InvalidOperationException">The <see cref="MediaDatabase"/> is disconnected.</exception>
        /// <exception cref="ObjectDisposedException">The <see cref="MediaDatabase"/> has already been disposed of.</exception>
        /// <exception cref="MediaDatabaseException">An error occurred while executing the command.</exception>
        public MediaDataReader<Playlist> Select()
        {
            return Select(null);
        }

        /// <summary>
        /// Retrieves the playlists with <see cref="SelectArguments"/>.
        /// </summary>
        /// <param name="filter">The criteria to use to filter. This value can be null.</param>
        /// <returns>The <see cref="MediaDataReader{TRecord}"/> containing the results.</returns>
        /// <exception cref="InvalidOperationException">The <see cref="MediaDatabase"/> is disconnected.</exception>
        /// <exception cref="ObjectDisposedException">The <see cref="MediaDatabase"/> has already been disposed of.</exception>
        /// <exception cref="MediaDatabaseException">An error occurred while executing the command.</exception>
        public MediaDataReader<Playlist> Select(SelectArguments filter)
        {
            ValidateDatabase();

            return CommandHelper.Select(filter, Interop.Playlist.ForeachPlaylistFromDb,
                Playlist.FromHandle);
        }

        /// <summary>
        /// Retrieves the playlist with the specified playlist id.
        /// </summary>
        /// <param name="playlistId">The playlist id to select.</param>
        /// <returns>The <see cref="Playlist"/> instance if the matched record was found in the database, otherwise null.</returns>
        /// <exception cref="InvalidOperationException">The <see cref="MediaDatabase"/> is disconnected.</exception>
        /// <exception cref="ObjectDisposedException">The <see cref="MediaDatabase"/> has already been disposed of.</exception>
        /// <exception cref="MediaDatabaseException">An error occurred while executing the command.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="playlistId"/> is less than or equal to zero.</exception>
        public Playlist Select(int playlistId)
        {
            ValidateDatabase();

            if (playlistId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(playlistId), playlistId,
                    "Playlist id can't be less than or equal to zero.");
            }

            IntPtr handle = IntPtr.Zero;

            try
            {
                Interop.Playlist.GetPlaylistFromDb(playlistId, out handle).ThrowIfError("Failed to query");

                if (handle == IntPtr.Zero)
                {
                    return null;
                }

                return new Playlist(handle);
            }
            finally
            {
                if (handle != IntPtr.Zero)
                {
                    Interop.Playlist.Destroy(handle);
                }
            }
        }

        /// <summary>
        /// Retrieves the number of media info of the playlist.
        /// </summary>
        /// <param name="playlistId">The playlist id to count media added to the playlist.</param>
        /// <returns>The number of media info.</returns>
        /// <exception cref="InvalidOperationException">The <see cref="MediaDatabase"/> is disconnected.</exception>
        /// <exception cref="ObjectDisposedException">The <see cref="MediaDatabase"/> has already been disposed of.</exception>
        /// <exception cref="MediaDatabaseException">An error occurred while executing the command.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="playlistId"/> is less than or equal to zero.</exception>
        public int CountMember(int playlistId)
        {
            return CountMember(playlistId, null);
        }

        /// <summary>
        /// Retrieves the number of media info of the playlist with <see cref="CountArguments"/>.
        /// </summary>
        /// <param name="playlistId">The playlist id to count media added to the playlist.</param>
        /// <param name="arguments">The criteria to use to filter. This value can be null.</param>
        /// <returns>The number of media info.</returns>
        /// <exception cref="InvalidOperationException">The <see cref="MediaDatabase"/> is disconnected.</exception>
        /// <exception cref="ObjectDisposedException">The <see cref="MediaDatabase"/> has already been disposed of.</exception>
        /// <exception cref="MediaDatabaseException">An error occurred while executing the command.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="playlistId"/> is less than or equal to zero.</exception>
        public int CountMember(int playlistId, CountArguments arguments)
        {
            ValidateDatabase();

            if (playlistId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(playlistId), playlistId,
                    "Playlist id can't be less than or equal to zero.");
            }

            return CommandHelper.Count(Interop.Playlist.GetMediaCountFromDb, playlistId, arguments);

        }

        private static List<PlaylistMember> GetMembers(int playlistId, SelectArguments arguments)
        {
            using (var filter = QueryArguments.ToNativeHandle(arguments))
            {
                Exception caught = null;
                List<PlaylistMember> list = new List<PlaylistMember>();

                Interop.Playlist.ForeachMediaFromDb(playlistId, filter, (memberId, mediaInfoHandle, _) =>
                {
                    try
                    {
                        list.Add(new PlaylistMember(memberId, MediaInfo.FromHandle(mediaInfoHandle)));

                        return true;
                    }
                    catch (Exception e)
                    {
                        caught = e;
                        return false;
                    }
                }).ThrowIfError("Failed to query");

                if (caught != null)
                {
                    throw caught;
                }

                return list;
            }
        }

        /// <summary>
        /// Retrieves the member id of the media in the playlist.
        /// </summary>
        /// <param name="playlistId">The playlist id.</param>
        /// <param name="mediaId">The media id.</param>
        /// <returns>The member id if the member was found in the playlist, otherwise -1.</returns>
        /// <exception cref="InvalidOperationException">The <see cref="MediaDatabase"/> is disconnected.</exception>
        /// <exception cref="ObjectDisposedException">The <see cref="MediaDatabase"/> has already been disposed of.</exception>
        /// <exception cref="MediaDatabaseException">An error occurred while executing the command.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="playlistId"/> is less than or equal to zero.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="mediaId"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="mediaId"/> is a zero-length string, contains only white space.</exception>
        public int GetMemberId(int playlistId, string mediaId)
        {
            ValidateDatabase();

            ValidationUtil.ValidateNotNullOrEmpty(mediaId, nameof(mediaId));

            if (playlistId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(playlistId), playlistId,
                    "Playlist id can't be less than or equal to zero.");
            }

            var reader = SelectMember(playlistId, new SelectArguments()
            {
                FilterExpression = $"{MediaInfoColumns.Id}='{mediaId}'"
            });
            reader.Read();

            return reader.Current?.MemberId ?? -1;
        }

        /// <summary>
        /// Retrieves the media info of the playlist.
        /// </summary>
        /// <param name="playlistId">The playlist id to query with.</param>
        /// <returns>The <see cref="MediaDataReader{TRecord}"/> containing the results.</returns>
        /// <exception cref="InvalidOperationException">The <see cref="MediaDatabase"/> is disconnected.</exception>
        /// <exception cref="ObjectDisposedException">The <see cref="MediaDatabase"/> has already been disposed of.</exception>
        /// <exception cref="MediaDatabaseException">An error occurred while executing the command.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="playlistId"/> is less than or equal to zero.</exception>
        public MediaDataReader<PlaylistMember> SelectMember(int playlistId)
        {
            return SelectMember(playlistId, null);
        }

        /// <summary>
        /// Retrieves the media info of the playlist with <see cref="SelectArguments"/>.
        /// </summary>
        /// <param name="playlistId">The playlist id to query with.</param>
        /// <param name="filter">The criteria to use to filter. This value can be null.</param>
        /// <returns>The <see cref="MediaDataReader{TRecord}"/> containing the results.</returns>
        /// <exception cref="InvalidOperationException">The <see cref="MediaDatabase"/> is disconnected.</exception>
        /// <exception cref="ObjectDisposedException">The <see cref="MediaDatabase"/> has already been disposed of.</exception>
        /// <exception cref="MediaDatabaseException">An error occurred while executing the command.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="playlistId"/> is less than or equal to zero.</exception>
        public MediaDataReader<PlaylistMember> SelectMember(int playlistId, SelectArguments filter)
        {
            ValidateDatabase();

            if (playlistId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(playlistId), playlistId,
                    "Playlist id can't be less than or equal to zero.");
            }

            return new MediaDataReader<PlaylistMember>(GetMembers(playlistId, filter));
        }

        /// <summary>
        /// Updates a playlist with the specified values.
        /// </summary>
        /// <privilege>http://tizen.org/privilege/content.write</privilege>
        /// <param name="playlistId">The playlist id to update.</param>
        /// <param name="values">The values for update.</param>
        /// <returns>true if the matched record was found and updated, otherwise false.</returns>
        /// <remarks>Only values set in <see cref="PlaylistUpdateValues"/> are updated.</remarks>
        /// <exception cref="InvalidOperationException">The <see cref="MediaDatabase"/> is disconnected.</exception>
        /// <exception cref="ObjectDisposedException">The <see cref="MediaDatabase"/> has already been disposed of.</exception>
        /// <exception cref="MediaDatabaseException">An error occurred while executing the command.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="values"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="playlistId"/> is less than or equal to zero.</exception>
        /// <exception cref="UnauthorizedAccessException">The caller has no required privilege.</exception>
        public bool Update(int playlistId, PlaylistUpdateValues values)
        {
            ValidateDatabase();

            if (playlistId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(playlistId), playlistId,
                    "Playlist id can't be less than or equal to zero.");
            }

            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            if (CommandHelper.Count(
                Interop.Playlist.GetPlaylistCount, $"{PlaylistColumns.Id}={playlistId}") == 0)
            {
                return false;
            }

            Interop.Playlist.Create(out var handle).ThrowIfError("Failed to update");

            try
            {
                if (values.Name != null)
                {
                    Interop.Playlist.SetName(handle, values.Name).ThrowIfError("Failed to update");
                }

                if (values.ThumbnailPath != null)
                {
                    Interop.Playlist.SetThumbnailPath(handle, values.ThumbnailPath).ThrowIfError("Failed to update");
                }

                Interop.Playlist.Update(playlistId, handle).ThrowIfError("Failed to update");
                return true;
            }
            finally
            {
                if (handle != IntPtr.Zero)
                {
                    Interop.Playlist.Destroy(handle);
                }
            }
        }

        /// <summary>
        /// Adds media to a playlist.
        /// </summary>
        /// <param name="playlistId">The playlist id that the media will be added to.</param>
        /// <param name="mediaId">The media id to add to the playlist.</param>
        /// <returns>true if the matched record was found and updated, otherwise false.</returns>
        /// <remarks>The invalid media id will be ignored.</remarks>
        /// <exception cref="InvalidOperationException">The <see cref="MediaDatabase"/> is disconnected.</exception>
        /// <exception cref="ObjectDisposedException">The <see cref="MediaDatabase"/> has already been disposed of.</exception>
        /// <exception cref="MediaDatabaseException">An error occurred while executing the command.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="mediaId"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="mediaId"/> is a zero-length string, contains only white space.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="playlistId"/> is less than or equal to zero.</exception>
        public bool AddMember(int playlistId, string mediaId)
        {
            ValidationUtil.ValidateNotNullOrEmpty(mediaId, nameof(mediaId));

            return AddMembers(playlistId, new string[] { mediaId });
        }

        /// <summary>
        /// Adds a media set to a playlist.
        /// </summary>
        /// <param name="playlistId">The playlist id that the media will be added to.</param>
        /// <param name="mediaIds">The collection of media id to add to the playlist.</param>
        /// <returns>true if the matched record was found and updated, otherwise false.</returns>
        /// <remarks>The invalid media ids will be ignored.</remarks>
        /// <exception cref="InvalidOperationException">The <see cref="MediaDatabase"/> is disconnected.</exception>
        /// <exception cref="ObjectDisposedException">The <see cref="MediaDatabase"/> has already been disposed of.</exception>
        /// <exception cref="MediaDatabaseException">An error occurred while executing the command.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="mediaIds"/> is null.</exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="mediaIds"/> has no element.\n
        ///     -or-\n
        ///     <paramref name="mediaIds"/> contains null value.\n
        ///     -or-\n
        ///     <paramref name="mediaIds"/> contains a zero-length string or white space.\n
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="playlistId"/> is less than or equal to zero.</exception>
        public bool AddMembers(int playlistId, IEnumerable<string> mediaIds)
        {
            ValidateDatabase();

            if (playlistId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(playlistId), playlistId,
                    "Playlist id can't be less than or equal to zero.");
            }

            if (mediaIds == null)
            {
                throw new ArgumentNullException(nameof(mediaIds));
            }

            if (mediaIds.Count() == 0)
            {
                throw new ArgumentException("mediaIds has no element.", nameof(mediaIds));
            }

            if (CommandHelper.Count(
                Interop.Playlist.GetPlaylistCount, $"{PlaylistColumns.Id}={playlistId}") == 0)
            {
                return false;
            }

            IntPtr handle = IntPtr.Zero;
            Interop.Playlist.Create(out handle).ThrowIfError("Failed to add member");

            try
            {
                foreach (var mediaId in mediaIds)
                {
                    if (mediaId == null)
                    {
                        throw new ArgumentException("Media id should not be null.", nameof(mediaIds));
                    }

                    if (string.IsNullOrWhiteSpace(mediaId))
                    {
                        throw new ArgumentException("Media id should not be empty.", nameof(mediaId));
                    }

                    Interop.Playlist.AddMedia(handle, mediaId).ThrowIfError("Failed to add member");
                }

                Interop.Playlist.Update(playlistId, handle).ThrowIfError("Failed to add member");
                return true;
            }
            finally
            {
                if (handle != IntPtr.Zero)
                {
                    Interop.Playlist.Destroy(handle);
                }
            }
        }

        /// <summary>
        /// Removes a member from a playlist.
        /// </summary>
        /// <param name="playlistId">The playlist id.</param>
        /// <param name="memberId">The member id to be removed.</param>
        /// <returns>true if the matched record was found and updated, otherwise false.</returns>
        /// <remarks>The invalid id will be ignored.</remarks>
        /// <exception cref="InvalidOperationException">The <see cref="MediaDatabase"/> is disconnected.</exception>
        /// <exception cref="ObjectDisposedException">The <see cref="MediaDatabase"/> has already been disposed of.</exception>
        /// <exception cref="MediaDatabaseException">An error occurred while executing the command.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="playlistId"/> is less than or equal to zero.\n
        ///     -or-\n
        ///     <paramref name="memberId"/> is less than or equal to zero.\n
        /// </exception>
        public bool RemoveMember(int playlistId, int memberId)
        {
            if (memberId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(memberId), memberId,
                    "Member id can't be less than or equal to zero.");
            }

            return RemoveMembers(playlistId, new int[] { memberId });
        }

        /// <summary>
        /// Removes a media set from a playlist.
        /// </summary>
        /// <param name="playlistId">The playlist id.</param>
        /// <param name="memberIds">The collection of member id to remove from to the playlist.</param>
        /// <returns>true if the matched record was found and updated, otherwise false.</returns>
        /// <remarks>The invalid ids will be ignored.</remarks>
        /// <exception cref="InvalidOperationException">The <see cref="MediaDatabase"/> is disconnected.</exception>
        /// <exception cref="ObjectDisposedException">The <see cref="MediaDatabase"/> has already been disposed of.</exception>
        /// <exception cref="MediaDatabaseException">An error occurred while executing the command.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="memberIds"/> is null.</exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="memberIds"/> has no element.\n
        ///     -or-\n
        ///     <paramref name="memberIds"/> contains a value which is less than or equal to zero.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="playlistId"/> is less than or equal to zero.</exception>
        public bool RemoveMembers(int playlistId, IEnumerable<int> memberIds)
        {
            ValidateDatabase();

            if (playlistId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(playlistId), playlistId,
                    "Playlist id can't be less than or equal to zero.");
            }

            if (memberIds == null)
            {
                throw new ArgumentNullException(nameof(memberIds));
            }

            if (memberIds.Count() == 0)
            {
                throw new ArgumentException("memberIds has no element.", nameof(memberIds));
            }

            if (CommandHelper.Count(
                Interop.Playlist.GetPlaylistCount, $"{PlaylistColumns.Id}={playlistId}") == 0)
            {
                return false;
            }

            IntPtr handle = IntPtr.Zero;
            Interop.Playlist.Create(out handle).ThrowIfError("Failed to add member");

            try
            {
                foreach (var memberId in memberIds)
                {
                    if (memberId <= 0)
                    {
                        throw new ArgumentException(nameof(memberIds),
                            "Member id can't be less than or equal to zero.");
                    }

                    Interop.Playlist.RemoveMedia(handle, memberId).ThrowIfError("Failed to add member");
                }

                Interop.Playlist.Update(playlistId, handle).ThrowIfError("Failed to add member");
                return true;
            }
            finally
            {
                if (handle != IntPtr.Zero)
                {
                    Interop.Playlist.Destroy(handle);
                }
            }
        }

        /// <summary>
        /// Updates a play order of a playlist.
        /// </summary>
        /// <param name="playlistId">The playlist id.</param>
        /// <param name="playOrder">The <see cref="PlayOrder"/> to apply.</param>
        /// <returns>true if the matched record was found and updated, otherwise false.</returns>
        /// <remarks>The <see cref="PlayOrder.MemberId"/> that is invalid will be ignored.</remarks>
        /// <exception cref="InvalidOperationException">The <see cref="MediaDatabase"/> is disconnected.</exception>
        /// <exception cref="ObjectDisposedException">The <see cref="MediaDatabase"/> has already been disposed of.</exception>
        /// <exception cref="MediaDatabaseException">An error occurred while executing the command.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="playOrder"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="playlistId"/> is less than or equal to zero.</exception>
        public bool UpdatePlayOrder(int playlistId, PlayOrder playOrder)
        {
            if (playOrder == null)
            {
                throw new ArgumentNullException(nameof(playOrder));
            }
            return UpdatePlayOrders(playlistId, new PlayOrder[] { playOrder });
        }

        /// <summary>
        /// Updates play orders of a playlist.
        /// </summary>
        /// <param name="playlistId">The playlist id.</param>
        /// <param name="orders">The collection of <see cref="PlayOrder"/> to apply.</param>
        /// <returns>true if the matched record was found and updated, otherwise false.</returns>
        /// <remarks>The <see cref="PlayOrder.MemberId"/> that is invalid will be ignored.</remarks>
        /// <exception cref="InvalidOperationException">The <see cref="MediaDatabase"/> is disconnected.</exception>
        /// <exception cref="ObjectDisposedException">The <see cref="MediaDatabase"/> has already been disposed of.</exception>
        /// <exception cref="MediaDatabaseException">An error occurred while executing the command.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="orders"/> is null.</exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="orders"/> has no element.\n
        ///     -or-\n
        ///     <paramref name="orders"/> contains a null value.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="playlistId"/> is less than or equal to zero.</exception>
        public bool UpdatePlayOrders(int playlistId, IEnumerable<PlayOrder> orders)
        {
            ValidateDatabase();

            if (playlistId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(playlistId), playlistId,
                    "Playlist id can't be less than or equal to zero.");
            }

            if (orders == null)
            {
                throw new ArgumentNullException(nameof(orders));
            }

            if (orders.Count() == 0)
            {
                throw new ArgumentException("memberIds has no element.", nameof(orders));
            }

            if (CommandHelper.Count(
                Interop.Playlist.GetPlaylistCount, $"{PlaylistColumns.Id}={playlistId}") == 0)
            {
                return false;
            }

            IntPtr handle = IntPtr.Zero;
            Interop.Playlist.Create(out handle).ThrowIfError("Failed to add member");

            try
            {
                foreach (var order in orders)
                {
                    if (order == null)
                    {
                        throw new ArgumentException(nameof(orders),
                            "orders should not contain null value.");
                    }
                    Interop.Playlist.SetPlayOrder(handle, order.MemberId, order.Value).ThrowIfError("Failed to add member");
                }

                Interop.Playlist.Update(playlistId, handle).ThrowIfError("Failed to add member");
                return true;
            }
            finally
            {
                if (handle != IntPtr.Zero)
                {
                    Interop.Playlist.Destroy(handle);
                }
            }
        }
    }
}