﻿/*
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FrameWork;

namespace Common
{
    [DataTable(PreCache = false, TableName = "Tok_Infos", DatabaseName = "World")]
    [Serializable]
    public class Tok_Info : DataObject
    {
        [PrimaryKey()]
        public uint Entry;

        [DataElement(Varchar = 255)]
        public string Name;

        [DataElement()]
        public uint Xp;

        [DataElement()]
        public uint Section;

        [DataElement()]
        public uint Index;

        [DataElement()]
        public uint Flag;

        [DataElement(Varchar = 255)]
        public string EventName;
    }
}