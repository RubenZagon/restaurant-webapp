import { useTranslation } from 'react-i18next'
import { colors } from '../../theme/colors'

export function LanguageSelector() {
  const { i18n, t } = useTranslation()

  const languages = [
    { code: 'en', label: t('language.en'), flag: '🇬🇧' },
    { code: 'es', label: t('language.es'), flag: '🇪🇸' }
  ]

  const changeLanguage = (languageCode: string) => {
    i18n.changeLanguage(languageCode)
  }

  return (
    <div style={{
      display: 'flex',
      alignItems: 'center',
      gap: '8px',
      padding: '8px 12px',
      backgroundColor: colors.background.paper,
      borderRadius: '8px',
      border: `1px solid ${colors.border.light}`
    }}>
      <span style={{
        fontSize: '14px',
        fontWeight: '500',
        color: colors.text.secondary,
        marginRight: '4px'
      }}>
        {t('language.select')}:
      </span>
      {languages.map((lang) => (
        <button
          key={lang.code}
          onClick={() => changeLanguage(lang.code)}
          style={{
            padding: '6px 12px',
            border: i18n.language === lang.code
              ? `2px solid ${colors.primary.main}`
              : `1px solid ${colors.border.main}`,
            borderRadius: '6px',
            backgroundColor: i18n.language === lang.code
              ? colors.primary.light
              : colors.background.paper,
            color: i18n.language === lang.code
              ? colors.primary.main
              : colors.text.primary,
            cursor: 'pointer',
            fontSize: '14px',
            fontWeight: i18n.language === lang.code ? '600' : '400',
            display: 'flex',
            alignItems: 'center',
            gap: '6px',
            transition: 'all 0.2s ease'
          }}
          onMouseEnter={(e) => {
            if (i18n.language !== lang.code) {
              e.currentTarget.style.backgroundColor = colors.neutral.lightGray
            }
          }}
          onMouseLeave={(e) => {
            if (i18n.language !== lang.code) {
              e.currentTarget.style.backgroundColor = colors.background.paper
            }
          }}
        >
          <span>{lang.flag}</span>
          <span>{lang.label}</span>
        </button>
      ))}
    </div>
  )
}
