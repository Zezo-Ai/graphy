/* ---------------------------------------
 * Author:          Martin Pane (martintayx@gmail.com) (@martinTayx)
 * Contributors:    https://github.com/Tayx94/graphy/graphs/contributors
 * Project:         Graphy - Ultimate Stats Monitor
 * Date:            03-Jan-18
 * Studio:          Tayx
 *
 * Git repo:        https://github.com/Tayx94/graphy
 *
 * This project is released under the MIT license.
 * Attribution is not required, but it is always welcomed!
 * -------------------------------------*/

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Tayx.Graphy.UI;
using Tayx.Graphy.Utils;

namespace Tayx.Graphy.Audio
{
    public class G_AudioManager : G_ModuleManager
    {
        #region Variables -> Serialized Private

        [SerializeField] private GameObject m_audioGraphGameObject = null;
        [SerializeField] private Text m_audioDbText = null;

        [SerializeField] private List<Image> m_backgroundImages = new List<Image>();

        #endregion

        #region Variables -> Private

        private G_AudioGraph m_audioGraph = null;
        private G_AudioMonitor m_audioMonitor = null;
        private G_AudioText m_audioText = null;

        #endregion

        #region Methods -> Unity Callbacks

        private void Awake()
        {
            Init();
        }

        private void Start()
        {
            UpdateParameters();
        }

        #endregion

        #region Methods -> Public

        public override void SetPosition( GraphyManager.ModulePosition newModulePosition, Vector2 offset )
        {
            m_audioDbText.alignment = TextAnchor.UpperRight;

            base.SetPosition( newModulePosition, offset );
        }

        public override void UpdateParameters()
        {
            foreach( var image in m_backgroundImages )
            {
                image.color = m_graphyManager.BackgroundColor;
            }

            m_audioGraph.UpdateParameters();
            m_audioMonitor.UpdateParameters();
            m_audioText.UpdateParameters();

            SetState( m_graphyManager.AudioModuleState );
        }

        public override void RefreshParameters()
        {
            foreach( var image in m_backgroundImages )
            {
                image.color = m_graphyManager.BackgroundColor;
            }

            m_audioGraph.UpdateParameters();
            m_audioMonitor.UpdateParameters();
            m_audioText.UpdateParameters();

            SetState( m_currentModuleState, true );
        }

        #endregion

        #region Methods -> Protected Override

        protected override void ApplyModuleState( GraphyManager.ModuleState state )
        {
            switch( state )
            {
                case GraphyManager.ModuleState.FULL:
                    gameObject.SetActive( true );
                    m_childrenGameObjects.SetAllActive( true );
                    SetGraphActive( true );

                    if( m_graphyManager.Background )
                    {
                        m_backgroundImages.SetOneActive( 0 );
                    }
                    else
                    {
                        m_backgroundImages.SetAllActive( false );
                    }

                    break;

                case GraphyManager.ModuleState.TEXT:
                case GraphyManager.ModuleState.BASIC:
                    gameObject.SetActive( true );
                    m_childrenGameObjects.SetAllActive( true );
                    SetGraphActive( false );

                    if( m_graphyManager.Background )
                    {
                        m_backgroundImages.SetOneActive( 1 );
                    }
                    else
                    {
                        m_backgroundImages.SetAllActive( false );
                    }

                    break;

                case GraphyManager.ModuleState.BACKGROUND:
                    gameObject.SetActive( true );
                    SetGraphActive( false );
                    m_childrenGameObjects.SetAllActive( false );

                    m_backgroundImages.SetAllActive( false );

                    break;

                case GraphyManager.ModuleState.OFF:
                    gameObject.SetActive( false );
                    break;
            }
        }

        #endregion

        #region Methods -> Private

        private void Init()
        {
            InitBase();

            m_audioGraph = GetComponent<G_AudioGraph>();
            m_audioMonitor = GetComponent<G_AudioMonitor>();
            m_audioText = GetComponent<G_AudioText>();
        }

        private void SetGraphActive( bool active )
        {
            m_audioGraph.enabled = active;
            m_audioGraphGameObject.SetActive( active );
        }

        #endregion
    }
}
