/* ---------------------------------------
 * Author:          Martin Pane (martintayx@gmail.com) (@martinTayx)
 * Contributors:    https://github.com/Tayx94/graphy/graphs/contributors
 * Project:         Graphy - Ultimate Stats Monitor
 * Date:            15-Dec-17
 * Studio:          Tayx
 *
 * Git repo:        https://github.com/Tayx94/graphy
 *
 * This project is released under the MIT license.
 * Attribution is not required, but it is always welcomed!
 * -------------------------------------*/

using UnityEngine;

namespace Tayx.Graphy.Fps
{
    public class G_FpsMonitor : MonoBehaviour
    {
        #region Variables -> Private

        private short[] m_fpsSamples;
        private short[] m_lowestSamples;
        private short m_fpsSamplesCapacity = 1024;
        private short m_onePercentSamples = 10;
        private short m_zero1PercentSamples = 1;
        private short m_fpsSamplesCount = 0;
        private short m_indexSample = 0;

        private float m_unscaledDeltaTime = 0f;

        private uint m_runningSum = 0;

        #endregion

        #region Properties -> Public

        public short CurrentFPS { get; private set; } = 0;
        public short AverageFPS { get; private set; } = 0;
        public short OnePercentFPS { get; private set; } = 0;
        public short Zero1PercentFps { get; private set; } = 0;

        #endregion

        #region Methods -> Unity Callbacks

        private void Awake()
        {
            Init();
        }

        private void Update()
        {
            m_unscaledDeltaTime = Time.unscaledDeltaTime;

            // Update fps and ms

            CurrentFPS = (short) (Mathf.RoundToInt( 1f / m_unscaledDeltaTime ));

            // Update avg fps

            m_indexSample++;

            if( m_indexSample >= m_fpsSamplesCapacity ) m_indexSample = 0;

            m_runningSum -= (uint) m_fpsSamples[ m_indexSample ];
            m_fpsSamples[ m_indexSample ] = CurrentFPS;
            m_runningSum += (uint) CurrentFPS;

            if( m_fpsSamplesCount < m_fpsSamplesCapacity )
            {
                m_fpsSamplesCount++;
            }

            AverageFPS = (short) ((float) m_runningSum / (float) m_fpsSamplesCount);

            // Update percent lows

            short k = m_fpsSamplesCount < m_onePercentSamples
                ? m_fpsSamplesCount
                : m_onePercentSamples;

            for( int i = 0; i < k; i++ )
            {
                m_lowestSamples[ i ] = short.MaxValue;
            }

            int startIdx = m_fpsSamplesCount < m_fpsSamplesCapacity ? 1 : 0;

            for( int i = startIdx; i < startIdx + m_fpsSamplesCount; i++ )
            {
                short sample = m_fpsSamples[ i ];

                if( sample < m_lowestSamples[ k - 1 ] )
                {
                    m_lowestSamples[ k - 1 ] = sample;

                    for( int j = k - 1; j > 0 && m_lowestSamples[ j ] < m_lowestSamples[ j - 1 ]; j-- )
                    {
                        short temp = m_lowestSamples[ j ];
                        m_lowestSamples[ j ] = m_lowestSamples[ j - 1 ];
                        m_lowestSamples[ j - 1 ] = temp;
                    }
                }
            }

            uint totalAddedFps = 0;

            short kZero1 = m_fpsSamplesCount < m_zero1PercentSamples
                ? m_fpsSamplesCount
                : m_zero1PercentSamples;

            for( int i = 0; i < k; i++ )
            {
                totalAddedFps += (ushort) m_lowestSamples[ i ];

                if( i == kZero1 - 1 )
                {
                    Zero1PercentFps = (short) ((float) totalAddedFps / (float) m_zero1PercentSamples);
                }
            }

            OnePercentFPS = (short) ((float) totalAddedFps / (float) m_onePercentSamples);
        }

        #endregion

        #region Methods -> Public

        public void UpdateParameters()
        {
            m_onePercentSamples = (short) (m_fpsSamplesCapacity / 100);
            m_zero1PercentSamples = (short) (m_fpsSamplesCapacity / 1000);
        }

        #endregion

        #region Methods -> Private

        private void Init()
        {
            m_fpsSamples = new short[m_fpsSamplesCapacity];

            UpdateParameters();

            m_lowestSamples = new short[m_onePercentSamples > 0 ? m_onePercentSamples : 1];
        }

        #endregion
    }
}