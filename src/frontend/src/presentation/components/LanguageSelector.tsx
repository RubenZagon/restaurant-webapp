import { useTranslation } from 'react-i18next'
import { colors } from '../../theme/colors'

export function LanguageSelector() {
  const { i18n } = useTranslation()

  const toggleLanguage = () => {
    const newLang = i18n.language === 'es' ? 'en' : 'es'
    i18n.changeLanguage(newLang)
  }

  const isSpanish = i18n.language === 'es'

  return (
    <button
      onClick={toggleLanguage}
      style={{
        position: 'relative',
        width: '64px',
        height: '32px',
        backgroundColor: colors.neutral.lightGray,
        borderRadius: '16px',
        border: `2px solid ${colors.border.main}`,
        cursor: 'pointer',
        transition: 'all 0.3s ease',
        padding: 0,
        boxShadow: 'inset 0 2px 4px rgba(0,0,0,0.1)'
      }}
      onMouseEnter={(e) => {
        e.currentTarget.style.borderColor = colors.primary.main
      }}
      onMouseLeave={(e) => {
        e.currentTarget.style.borderColor = colors.border.main
      }}
      aria-label="Toggle language"
      title={isSpanish ? 'Switch to English' : 'Cambiar a Español'}
    >
      {/* Switch slider */}
      <div style={{
        position: 'absolute',
        top: '2px',
        left: isSpanish ? '2px' : 'calc(100% - 26px)',
        width: '24px',
        height: '24px',
        backgroundColor: colors.background.paper,
        borderRadius: '50%',
        transition: 'all 0.3s cubic-bezier(0.4, 0, 0.2, 1)',
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        fontSize: '14px',
        boxShadow: '0 2px 4px rgba(0,0,0,0.2)'
      }}>
        {isSpanish ? '🇪🇸' : '🇬🇧'}
      </div>

      {/* Background labels */}
      <div style={{
        position: 'absolute',
        top: '50%',
        left: isSpanish ? 'calc(100% - 22px)' : '6px',
        transform: 'translateY(-50%)',
        fontSize: '11px',
        opacity: 0.5,
        fontWeight: '600',
        color: colors.text.secondary,
        pointerEvents: 'none',
        transition: 'all 0.3s ease'
      }}>
        {isSpanish ? 'EN' : 'ES'}
      </div>
    </button>
  )
}
