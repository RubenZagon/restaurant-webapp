import { useTranslation } from 'react-i18next'
import { useState } from 'react'
import { colors } from '../../theme/colors'

export function LanguageSelector() {
  const { i18n, t } = useTranslation()
  const [hoveredLang, setHoveredLang] = useState<string | null>(null)

  const languages = [
    { code: 'en', label: t('language.en'), flag: '🇬🇧', emoji: '🍔' },
    { code: 'es', label: t('language.es'), flag: '🇪🇸', emoji: '🥘' }
  ]

  const changeLanguage = (languageCode: string) => {
    i18n.changeLanguage(languageCode)
  }

  return (
    <div style={{
      display: 'flex',
      alignItems: 'center',
      gap: '8px',
      padding: '10px 16px',
      background: `linear-gradient(135deg, ${colors.background.paper} 0%, ${colors.neutral.lightGray} 100%)`,
      borderRadius: '16px',
      border: `2px solid ${colors.border.light}`,
      boxShadow: '0 4px 12px rgba(0,0,0,0.08)',
      transition: 'all 0.3s cubic-bezier(0.4, 0, 0.2, 1)'
    }}>
      <span style={{
        fontSize: '20px',
        marginRight: '4px',
        animation: 'pulse 2s infinite'
      }}>
        🌍
      </span>
      <span style={{
        fontSize: '13px',
        fontWeight: '600',
        color: colors.text.secondary,
        marginRight: '4px',
        letterSpacing: '0.5px',
        textTransform: 'uppercase'
      }}>
        {t('language.select')}
      </span>
      {languages.map((lang) => {
        const isActive = i18n.language === lang.code
        const isHovered = hoveredLang === lang.code

        return (
          <button
            key={lang.code}
            onClick={() => changeLanguage(lang.code)}
            onMouseEnter={() => setHoveredLang(lang.code)}
            onMouseLeave={() => setHoveredLang(null)}
            style={{
              padding: '8px 16px',
              border: isActive
                ? `2px solid ${colors.primary.main}`
                : `2px solid transparent`,
              borderRadius: '12px',
              background: isActive
                ? `linear-gradient(135deg, ${colors.primary.main} 0%, ${colors.secondary.main} 100%)`
                : isHovered
                  ? colors.neutral.lightGray
                  : colors.background.paper,
              color: isActive
                ? colors.text.inverse
                : colors.text.primary,
              cursor: 'pointer',
              fontSize: '14px',
              fontWeight: isActive ? '700' : '500',
              display: 'flex',
              alignItems: 'center',
              gap: '8px',
              transition: 'all 0.4s cubic-bezier(0.4, 0, 0.2, 1)',
              transform: isActive ? 'scale(1.05)' : isHovered ? 'scale(1.02)' : 'scale(1)',
              boxShadow: isActive
                ? '0 6px 16px rgba(59, 130, 246, 0.4)'
                : isHovered
                  ? '0 4px 12px rgba(0,0,0,0.1)'
                  : '0 2px 4px rgba(0,0,0,0.05)',
              position: 'relative',
              overflow: 'hidden'
            }}
          >
            {isActive && (
              <span style={{
                position: 'absolute',
                top: '2px',
                right: '2px',
                fontSize: '10px',
                animation: 'bounce 1s infinite'
              }}>
                ✨
              </span>
            )}
            <span style={{
              fontSize: '20px',
              transition: 'transform 0.3s ease',
              transform: isHovered ? 'rotate(10deg) scale(1.2)' : 'rotate(0deg) scale(1)',
              display: 'inline-block'
            }}>
              {lang.flag}
            </span>
            <span style={{
              transition: 'all 0.3s ease',
              fontWeight: isActive ? '700' : '500'
            }}>
              {lang.label}
            </span>
            {isHovered && !isActive && (
              <span style={{
                fontSize: '14px',
                opacity: 0.7
              }}>
                {lang.emoji}
              </span>
            )}
          </button>
        )
      })}
      <style>{`
        @keyframes pulse {
          0%, 100% {
            transform: scale(1);
          }
          50% {
            transform: scale(1.1);
          }
        }
        @keyframes bounce {
          0%, 100% {
            transform: translateY(0);
          }
          50% {
            transform: translateY(-3px);
          }
        }
      `}</style>
    </div>
  )
}
